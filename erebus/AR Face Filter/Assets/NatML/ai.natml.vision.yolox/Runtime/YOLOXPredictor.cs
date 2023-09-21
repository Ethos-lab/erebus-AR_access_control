/* 
*   YOLOX
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Vision
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using NatML.Features;
    using NatML.Internal;
    using NatML.Types;
    using SortCS;
    using System.Drawing;
    using System.Diagnostics;

    /// <summary>
    /// YOLOX predictor for general object detection.
    /// This predictor accepts an image feature and produces a list of detections.
    /// Each detection is comprised of a normalized rect, label, and detection score.
    /// </summary>
    //public sealed class YOLOXPredictor : IMLPredictor<(int, Rect rect, string label, float score)[]>
    public sealed class YOLOXPredictor : IMLPredictor2<Bbox[]>, IMLPredictor<(int, Rect rect, string label, float score)[]>
    {

        #region --Client API--
        /// <summary>
        /// Class labels.
        /// </summary>
        public readonly string[] labels;
        //public readonly Dictionary<int, string> ToStringLabel = new Dictionary<int, string>();

        /// <summary>
        /// Create the YOLOX predictor.
        /// </summary>
        /// <param name="model">YOLOX ML model.</param>
        /// <param name="labels">Classification labels.</param>
        /// <param name="minScore">Minimum candidate score.</param>
        /// <param name="maxIoU">Maximum intersection-over-union score for overlap removal.</param>
        //public YOLOXPredictor (MLModel model, string[] labels, float minScore = 0.4f, float maxIoU = 0.5f) {

        public YOLOXPredictor(MLModel model, string[] labels, float minScore = 0.25f, float maxIoU = 0.45f)
        {
            //InitializeIntLabelConvertDict(labels);
            this.model = model as MLEdgeModel;
            this.labels = labels;
            this.minScore = minScore;
            this.maxIoU = maxIoU;
            this.inputType = model.inputs[0] as MLImageType;
            conflationManager = new ConflationManager(
            //whiteListObjectList: ConvertToIntLabels(whiteListObjectList),
            frameCount: 2,
            conflationThresh: 0.75f);

            trackManager = new TrackerManager(iouThreshold: 0.3f, maxMisses: 5);
        }
        //private void InitializeIntLabelConvertDict(string[] modelLabels)
        //{
        //    int clsIndex = 0;
        //    foreach (var modelLabel in modelLabels)
        //        ToStringLabel[clsIndex++] = modelLabel;
        //}
        private List<int> ConvertToIntLabels(List<string> whiteListObjectStringList)
        {
            var whiteListObjectIntList = new List<int>(whiteListObjectStringList.Count);
            foreach (var whiteListObject in whiteListObjectStringList)
            {
                bool noMatch = true;
                for (int i = 0; i < labels.Length; i++)
                {
                    var label = labels[i];
                    if (label == whiteListObject)
                    {
                        whiteListObjectIntList.Add(i);
                        noMatch = false;
                        break;
                    }
                }
                if (noMatch)
                    throw new Exception($"No corresponding class ({whiteListObject}) was found");
            }

            return whiteListObjectIntList;
        }

        /// <summary>
        /// Detect objects in an image.
        /// </summary>
        /// <param name="inputs">Input image.</param>
        /// <returns>Detected objects.</returns>
        //public unsafe (int, Rect rect, string label, float score)[] Predict(params MLFeature[] inputs)
        public unsafe (Bbox[], string[]) Predict2(params MLFeature[] inputs)
        {
            // Check
            if (inputs.Length != 1)
                throw new ArgumentException(@"YOLOX predictor expects a single feature", nameof(inputs));
            // Check type
            var input = inputs[0];
            var imageType = MLImageType.FromType(input.type);
            var imageFeature = input as MLImageFeature;
            if (!imageType)
                throw new ArgumentException(@"YOLOX predictor expects an an array or image feature", nameof(inputs));
            // Predict
            using var inputFeature = (input as IMLEdgeFeature).Create(inputType);
            using var outputFeatures = model.Predict(inputFeature);
            //Raw model output shape : (1,2100,85)
            // Marshal
            var logitsData = (float*)outputFeatures[0].data;      // (1,6300,85)
            var shape8 = new[] { inputType.height / 8, inputType.width / 8, 85 };
            var shape16 = new[] { inputType.height / 16, inputType.width / 16, 85 };
            var shape32 = new[] { inputType.height / 32, inputType.width / 32, 85 };
            var logits8 = new MLArrayFeature<float>(&logitsData[0], shape8);
            var logits16 = new MLArrayFeature<float>(&logitsData[logits8.elementCount], shape16);
            var logits32 = new MLArrayFeature<float>(&logitsData[logits8.elementCount + logits16.elementCount], shape32);
            var (widthInv, heightInv) = (1f / inputType.width, 1f / inputType.height);
            var candidateBoxes = new List<Rect>();
            var candidateScores = new List<float>();
            var candidateRawScores = new List<float[]>(); //80 classes
            var candidateLabels = new List<string>();
            foreach (var (logits, stride) in new[] { (logits8, 8), (logits16, 16), (logits32, 32) })
                for (int j = 0, jlen = logits.shape[0], ilen = logits.shape[1]; j < jlen; ++j)
                    for (int i = 0; i < ilen; ++i)
                    {
                        var score = logits[j, i, 4];

                        float maxScore = 0;
                        int maxScoreIndex = 5;
                        float[] rawScores = new float[80];
                        for (int k = 5, rawScoreIndex = 0; k < 85; k++, rawScoreIndex++)
                        {
                            var curScore = score * logits[j, i, k];
                            if (curScore > maxScore)
                            {
                                maxScore = logits[j, i, k];
                                maxScoreIndex = k;
                            }
                            rawScores[rawScoreIndex] = curScore;
                        }
                        var label = maxScoreIndex - 5;
                        score = rawScores[label];
                        if (score < minScore)
                            continue;

                        //var label = enumRange_5_80.Aggregate(
                        //    (p, q) => ((score * logits[j, i, p]) > (score * logits[j, i, q])) ? p : q) - 5;
                        //Debug.Log($"Score:{score} | CLASS:{labels[label]}({logits[j, i, label + 5]}) |" +
                        //    $" Mult:{multi_score}");
                        //score *= logits[j, i, label + 5];

                        // Decode box
                        var cx = (i + logits[j, i, 0]) * stride * widthInv;
                        var cy = 1f - (j + logits[j, i, 1]) * stride * heightInv;
                        var w = Mathf.Exp(logits[j, i, 2]) * stride * widthInv;
                        var h = Mathf.Exp(logits[j, i, 3]) * stride * heightInv;
                        var rawBox = new Rect(cx - 0.5f * w, cy - 0.5f * h, w, h);
                        var box = imageFeature?.TransformRect(rawBox, inputType) ?? rawBox;
                        //Debug.Log($"BOX : ({box.x},{box.y},{box.width},{box.height})");
                        // Add
                        candidateBoxes.Add(box);
                        candidateScores.Add(score);
                        candidateLabels.Add(labels[label]);
                        candidateRawScores.Add(rawScores);

                        //// Check
                        //var score = logits[j,i,4];
                        //if (score < minScore)
                        //    continue;
                        //// Get class
                        //var label = Enumerable
                        //    .Range(5, 80)
                        //    .Aggregate((p, q) => logits[j,i,p] > logits[j,i,q] ? p : q) - 5;
                        //Debug.Log($"CLASS:{logits[j, i, label+5]} | " +
                        //    $"Score:{score} | Mult:{logits[j, i, label + 5]* score}");
                        //score *= logits[j, i, label + 5];
                        //// Decode box
                        //var cx = (i + logits[j,i,0]) * stride * widthInv;
                        //var cy = 1f - (j + logits[j,i,1]) * stride * heightInv;
                        //var w = Mathf.Exp(logits[j,i,2]) * stride * widthInv;
                        //var h = Mathf.Exp(logits[j,i,3]) * stride * heightInv;
                        //var rawBox = new Rect(cx - 0.5f * w, cy - 0.5f * h, w, h);
                        //var box = imageFeature?.TransformRect(rawBox, inputType) ?? rawBox;
                        //// Add
                        //candidateBoxes.Add(box);
                        //candidateScores.Add(score);
                        //candidateLabels.Add(labels[label]);
                    }
            
            var keepIdx = MLImageFeature.NonMaxSuppression(candidateBoxes, candidateScores, maxIoU);
            //var result = new List<(int, Rect, string, float)>();
            var resultArg1 = new List<Bbox>();
            var resultArg2 = new List<string>();

            //Object Tracker
            trackManager.ResetNewTracks();
            foreach (var idx in keepIdx)
            {
                var bbox = candidateBoxes[idx];
                var label = candidateLabels[idx];
                var rawScore = candidateRawScores[idx];
                trackManager.AddNewTrack(bbox, rawScore, label);
            }
            var activeTrackDataList = trackManager.UpdateTracker();

            //Conflation
            foreach (var activeTrack in activeTrackDataList)
            {
                var id = activeTrack.TrackId;
                var label = activeTrack.Label;
                var rawScore = activeTrack.RawScore;
                var bbox = activeTrack.Bbox;

                (var pass, var cls, var score) = conflationManager.UpdateData(id, rawScore);
                if (pass)
                {
                    var formattedBbox = new Bbox((int)(imageType.width * bbox.X), (int)(imageType.height * bbox.Y),
                        (int)(imageType.width * bbox.Width), (int)(imageType.height * bbox.Height));
                    //var formattedBbox = new Bbox((int)(imageType.height * bbox.X), (int)(imageType.width * bbox.Y),
                    //    (int)(imageType.height * bbox.Width), (int)(imageType.width * bbox.Height));
                    resultArg1.Add(formattedBbox);
                    resultArg2.Add(label);
                }
            }

            // Return
            return (resultArg1.ToArray(), resultArg2.ToArray());
        }
        public unsafe (int, Rect rect, string label, float score)[] Predict(params MLFeature[] inputs)
        {
            // Check
            if (inputs.Length != 1)
                throw new ArgumentException(@"YOLOX predictor expects a single feature", nameof(inputs));
            // Check type
            var input = inputs[0];
            var imageType = MLImageType.FromType(input.type);
            var imageFeature = input as MLImageFeature;
            if (!imageType)
                throw new ArgumentException(@"YOLOX predictor expects an an array or image feature", nameof(inputs));
            // Predict
            using var inputFeature = (input as IMLEdgeFeature).Create(inputType);
            using var outputFeatures = model.Predict(inputFeature);
            //Raw model output shape : (1,2100,85)
            // Marshal
            var logitsData = (float*)outputFeatures[0].data;      // (1,6300,85)
            var shape8 = new[] { inputType.height / 8, inputType.width / 8, 85 };
            var shape16 = new[] { inputType.height / 16, inputType.width / 16, 85 };
            var shape32 = new[] { inputType.height / 32, inputType.width / 32, 85 };
            var logits8 = new MLArrayFeature<float>(&logitsData[0], shape8);
            var logits16 = new MLArrayFeature<float>(&logitsData[logits8.elementCount], shape16);
            var logits32 = new MLArrayFeature<float>(&logitsData[logits8.elementCount + logits16.elementCount], shape32);
            var (widthInv, heightInv) = (1f / inputType.width, 1f / inputType.height);
            var candidateBoxes = new List<Rect>();
            var candidateScores = new List<float>();
            var candidateRawScores = new List<float[]>(); //80 classes
            var candidateLabels = new List<string>();
            foreach (var (logits, stride) in new[] { (logits8, 8), (logits16, 16), (logits32, 32) })
                for (int j = 0, jlen = logits.shape[0], ilen = logits.shape[1]; j < jlen; ++j)
                    for (int i = 0; i < ilen; ++i)
                    {
                        var score = logits[j, i, 4];

                        float maxScore = 0;
                        int maxScoreIndex = 5;
                        float[] rawScores = new float[80];
                        for (int k = 5, rawScoreIndex = 0; k < 85; k++, rawScoreIndex++)
                        {
                            var curScore = score * logits[j, i, k];
                            if (curScore > maxScore)
                            {
                                maxScore = logits[j, i, k];
                                maxScoreIndex = k;
                            }
                            rawScores[rawScoreIndex] = curScore;
                        }
                        var label = maxScoreIndex - 5;
                        score = rawScores[label];
                        if (score < minScore)
                            continue;

                        //var label = enumRange_5_80.Aggregate(
                        //    (p, q) => ((score * logits[j, i, p]) > (score * logits[j, i, q])) ? p : q) - 5;
                        //Debug.Log($"Score:{score} | CLASS:{labels[label]}({logits[j, i, label + 5]}) |" +
                        //    $" Mult:{multi_score}");
                        //score *= logits[j, i, label + 5];

                        // Decode box
                        var cx = (i + logits[j, i, 0]) * stride * widthInv;
                        var cy = 1f - (j + logits[j, i, 1]) * stride * heightInv;
                        var w = Mathf.Exp(logits[j, i, 2]) * stride * widthInv;
                        var h = Mathf.Exp(logits[j, i, 3]) * stride * heightInv;
                        var rawBox = new Rect(cx - 0.5f * w, cy - 0.5f * h, w, h);
                        var box = imageFeature?.TransformRect(rawBox, inputType) ?? rawBox;
                        // Add
                        candidateBoxes.Add(box);
                        candidateScores.Add(score);
                        candidateLabels.Add(labels[label]);
                        candidateRawScores.Add(rawScores);

                        //// Check
                        //var score = logits[j,i,4];
                        //if (score < minScore)
                        //    continue;
                        //// Get class
                        //var label = Enumerable
                        //    .Range(5, 80)
                        //    .Aggregate((p, q) => logits[j,i,p] > logits[j,i,q] ? p : q) - 5;
                        //Debug.Log($"CLASS:{logits[j, i, label+5]} | " +
                        //    $"Score:{score} | Mult:{logits[j, i, label + 5]* score}");
                        //score *= logits[j, i, label + 5];
                        //// Decode box
                        //var cx = (i + logits[j,i,0]) * stride * widthInv;
                        //var cy = 1f - (j + logits[j,i,1]) * stride * heightInv;
                        //var w = Mathf.Exp(logits[j,i,2]) * stride * widthInv;
                        //var h = Mathf.Exp(logits[j,i,3]) * stride * heightInv;
                        //var rawBox = new Rect(cx - 0.5f * w, cy - 0.5f * h, w, h);
                        //var box = imageFeature?.TransformRect(rawBox, inputType) ?? rawBox;
                        //// Add
                        //candidateBoxes.Add(box);
                        //candidateScores.Add(score);
                        //candidateLabels.Add(labels[label]);
                    }
            var keepIdx = MLImageFeature.NonMaxSuppression(candidateBoxes, candidateScores, maxIoU);
            var result = new List<(int, Rect, string, float)>();

            //Object Tracker
            trackManager.ResetNewTracks();
            foreach (var idx in keepIdx)
            {
                var bbox = candidateBoxes[idx];
                var label = candidateLabels[idx];
                var rawScore = candidateRawScores[idx];
                trackManager.AddNewTrack(bbox, rawScore, label);
            }
            var activeTrackDataList = trackManager.UpdateTracker();

            //Conflation
            foreach (var activeTrack in activeTrackDataList)
            {
                var id = activeTrack.TrackId;
                var label = activeTrack.Label;
                var rawScore = activeTrack.RawScore;
                var bbox = activeTrack.Bbox;

                (var pass, var cls, var score) = conflationManager.UpdateData(id, rawScore);
                if (pass)
                    result.Add((id, new Rect(bbox.X, bbox.Y, bbox.Width, bbox.Height), labels[cls], score));
            }

            // Return
            return result.ToArray();
        }
        #endregion
        private ConflationManager conflationManager = null;
        private TrackerManager trackManager = null;

        #region --Operations--
        private readonly MLEdgeModel model;
        private readonly float minScore;
        private readonly float maxIoU;
        private readonly MLImageType inputType;

        void IDisposable.Dispose() { } // Not used
        #endregion
    }
    public class TrackerManager
    {
        private ITracker tracker = null;
        private List<TrackData> newTrackDataList = new List<TrackData>();
        public TrackerManager(float iouThreshold = 0.3f, int maxMisses = 3)
        {
            tracker = new SortTracker(iouThreshold, maxMisses);
        }
        public void ResetNewTracks()
        {
            newTrackDataList.Clear();
        }
        public void AddNewTrack(float x, float y, float w, float h, float[] rawScore, string label)
        {
            newTrackDataList.Add(new TrackData()
            {
                Bbox = new RectangleF(x, y, w, h),
                RawScore = rawScore,
                Label = label
            });
        }
        public void AddNewTrack(Rect rect, float[] rawScore, string label)
        {
            newTrackDataList.Add(new TrackData()
            {
                Bbox = new RectangleF(rect.x, rect.y, rect.width, rect.height),
                RawScore = rawScore,
                Label = label
            });
        }
        public IEnumerable<Track> UpdateTracker()
        {
            return tracker.Track(newTrackDataList);
        }
    }
    public class ConflationManager
    {
        //(K,V) : (TrackId, Score of 80 classes)
        private Dictionary<int, ObjectConflationData> data = new Dictionary<int, ObjectConflationData>();
        private int frameCount;
        private float conflationThresh;
        //private List<int> whiteListObjectList = null;

        public ConflationManager(int frameCount = 2, float conflationThresh = 0.75f)
        {
            this.frameCount = frameCount;
            //this.whiteListObjectList = whiteListObjectList;
            //The actual 'conflationThresh' must be computed ([0.8*0.8]) = 0.9
            this.conflationThresh = ComputeSingleConflate(frameCount, conflationThresh);
        }

        // (int, float) : (Max score class, Max score, raw scores)
        public (bool, int, float) UpdateData(int id, float[] rawScore)
        {
            if (!data.ContainsKey(id))
                data[id] = new ObjectConflationData(frameCount);

            data[id].Push(rawScore);
            (var cls, var score) = data[id].Peek();

            //Debug.Log($"{cls}, {score}, {whiteListObjectList.Contains(cls)}");
            //return (score > conflationThresh, cls, score);
            return (score > conflationThresh, cls, score);

            //return ((score > conflationThresh) && whiteListObjectList.Contains(cls), cls, score);
        }
        public float ComputeSingleConflate(int bufferSize, float singleThresh)
        {
            float[] probList = new float[bufferSize];
            for (int i = 0; i < probList.Length; i++)
                probList[i] = singleThresh;

            float scoreMulti = 1;
            float invScoreMulti = 1;
            for (int i = 0; i < probList.Length; i++)
            {
                var score = probList[i];
                var invScore = 1 - score;
                scoreMulti *= score;
                invScoreMulti *= invScore;
            }

            return scoreMulti / (scoreMulti + invScoreMulti);
        }
    }
    public class ObjectConflationData
    {
        //private static float[] emptyList = new float[80];
        private List<float[]> scoreList = null;
        private int frameBufferSize = 1;
        private int curFrameIndex = 0;
        private bool isFull = false;

        public ObjectConflationData(int frameCnt)
        {
            frameBufferSize = frameCnt;
            curFrameIndex = 0;
            scoreList = new List<float[]>(frameBufferSize);
            for (int i = 0; i < frameBufferSize; i++)
                scoreList.Add(null);
        }
        public void Push(float[] scores)
        {
            //Debug.Log($"{scoreList.Count}, {scoreList.Capacity}, {curFrameIndex}");
            scoreList[curFrameIndex] = scores;
            curFrameIndex += 1;

            if (curFrameIndex >= frameBufferSize)
            {
                curFrameIndex %= frameBufferSize;
                isFull = true;
            }
        }
        public (int, float) Peek()
        {
            if (frameBufferSize <= 1)
                throw new Exception("Framebuffer size must be larger than 1. Code is optimized just for > 1");

            return NaiveBayesConflate();
        }
        private (int, float) NaiveBayesConflate()
        {
            if (!isFull)
                return (0, 0);

            float maxScore = 0;
            int maxScoreClass = 0;
            //float[] conflationRes = new float[80];
            for (int classIndex = 0; classIndex < 80; classIndex++)
            {
                float scoreMulti = 1;
                float invScoreMulti = 1;
                for (int frameIndex = 0; frameIndex < frameBufferSize; frameIndex++)
                {
                    var score = scoreList[frameIndex][classIndex];
                    var invScore = 1 - score;
                    scoreMulti *= score;
                    invScoreMulti *= invScore;
                }
                var conflationScore = scoreMulti / (scoreMulti + invScoreMulti);
                if (conflationScore > maxScore)
                {
                    maxScore = conflationScore;
                    maxScoreClass = classIndex;
                }
                //conflationRes[classIndex] = conflationScore;
            }

            return (maxScoreClass, maxScore);
        }
    }
}
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgcodecsModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.ObjdetectModule;
using OpenCVForUnity.UnityUtils;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rect = OpenCVForUnity.CoreModule.Rect;

/// <summary>
/// FaceRecognizerSF Example
/// An example of human face recognition using the FaceRecognizerSF class.
/// https://github.com/opencv/opencv/blob/master/samples/dnn/face_detect.cpp
/// https://docs.opencv.org/4.5.4/d0/dd4/tutorial_dnn_face.html
/// </summary>
public class FaceRecognize
{
    //private const float scoreThreshold = 0.25f; // Filter out faces of score < score_threshold.
    private const float scoreThreshold = 0.5f; // Filter out faces of score < score_threshold.
    //private const float scoreThreshold = 0.9f; // Filter out faces of score < score_threshold.
    private const float nmsThreshold = 0.3f; // Suppress bounding boxes of iou >= nms_threshold
    private const int topK = 5000; // Keep top_k bounding boxes before NMS.
    private const double cosine_similar_thresh = 0.363; // The cosine similar thresh.
    private const double l2norm_similar_thresh = 1.128; // The l2norm similar thresh.
    //private const int imageSizeW = 112;
    //private const int imageSizeH = 112;
    private const int imageSizeW = 1000;
    private const int imageSizeH = 1000;

    private readonly string faceDetectModelName = @"face_detection_yunet_2021dec.onnx"; // Face detection (Bbox)
    private readonly string faceRecogModelName = @"face_recognition_sface_2021dec.onnx"; // Face recognition
    private FaceDetectorYN faceDetector = null;
    private FaceRecognizerSF faceRecognizer = null;

    public FaceRecognize()
    {
        var faceDetectModelPath = Utils.getFilePath(faceDetectModelName);
        var faceRecogModelPath = Utils.getFilePath(faceRecogModelName);

        faceDetector = FaceDetectorYN.create(faceDetectModelPath, "", new Size(imageSizeW, imageSizeH), scoreThreshold, nmsThreshold, topK);
        faceRecognizer = FaceRecognizerSF.create(faceRecogModelPath, "");
    }
    public (bool, double, double) FaceDetectAndRecognize(Mat inputImgMat, Mat cmpImgMat)
    {
        //var saveJpgPath = @"C:\Users\YoonsangKim\AppData\LocalLow\Yoonsang\Erebus Permission Manager\Erebus\inputImgMat.jpg";
        //Imgcodecs.imwrite(saveJpgPath, inputImgMat);

        Mat inputFaceFeatureMat = DetectAndExtractFeatures(inputImgMat);
        if (inputFaceFeatureMat == null)
        {
            Debug.Log($"[Input] Face not found");
            return (false, default, default);
        }

        Mat cmpFaceFeatureMat = DetectAndExtractFeatures(cmpImgMat); // Detect and extract facial features
        if (cmpFaceFeatureMat == null)
        {
            Debug.Log($"[Cmp] Face not found");
            return (false, default, default);
        }

        (var cosScore, var L2score, var match) = MatchFeatures(inputFaceFeatureMat, cmpFaceFeatureMat); // Match facial features
        Debug.Log($"Match : [{match}] ({cosScore}, {L2score})");

        return (match, cosScore, L2score);
    }
    private void SaveRoiRect(string savePath, Mat originImgMat, Rect roiRect)
    {
        var finalMat = new Mat(originImgMat, roiRect);
        Imgproc.cvtColor(finalMat, finalMat, Imgproc.COLOR_BGR2GRAY);
        Imgcodecs.imwrite(savePath, finalMat);
    }
    private (double, double, bool) MatchFeatures(Mat inputFeatureMat, Mat cmpFeatureMat)
    {
        double cos_score = faceRecognizer.match(inputFeatureMat, cmpFeatureMat, FaceRecognizerSF.FR_COSINE);
        bool cos_match = (cos_score >= cosine_similar_thresh);

        //Debug.Log((cos_match ? "[Same-1]" : "[Different-1]") + "\n"
        //    + " Cosine Similarity: " + cos_score + ", threshold: " + cosine_similar_thresh + ". (higher value means higher similarity, max 1.0)");


        double L2_score = faceRecognizer.match(inputFeatureMat, cmpFeatureMat, FaceRecognizerSF.FR_NORM_L2);
        bool L2_match = (L2_score <= l2norm_similar_thresh);

        //Debug.Log((L2_match ? "[Same-2]" : "[Different-2]") + "\n"
        //    + " NormL2 Distance: " + L2_score + ", threshold: " + l2norm_similar_thresh + ". (lower value means higher similarity, min 0.0)");

        return (cos_score, L2_score, (cos_match && L2_match));
    }
    private Mat DetectAndExtractFeatures(Mat imgMat)
    {
        faceDetector.setInputSize(imgMat.size());
        Mat faceMat = new Mat();
        faceDetector.detect(imgMat, faceMat);
        if (faceMat.rows() < 1)
        {
            //Debug.Log("Face not found!");
            return null;
        }

        // Aligning and cropping facial image through the first face of faces detected.
        Mat alignedInputFaceMat = AlignCrop(imgMat, faceMat);

        // Run feature extraction with given aligned_face.
        Mat inputImgFeatureMat = ExtractFeatures(alignedInputFaceMat);

        return inputImgFeatureMat;
    }
    private Mat AlignCrop(Mat imgMat, Mat faceMat)
    {
        Mat alignedFaceMat = new Mat();
        faceRecognizer.alignCrop(imgMat, faceMat.row(0), alignedFaceMat);
        return alignedFaceMat;
    }
    private Mat ExtractFeatures(Mat mat)
    {
        Mat featureMat = new Mat();
        faceRecognizer.feature(mat, featureMat);
        return featureMat.clone();
    }
    private void DrawDetection(Mat d, Mat frame)
    {
        float[] buf = new float[15];
        d.get(0, 0, buf);

        Scalar color = new Scalar(255, 255, 0, 255);

        Imgproc.rectangle(frame, new Point(buf[0], buf[1]), new Point(buf[0] + buf[2], buf[1] + buf[3]), color, 2);
        Imgproc.circle(frame, new Point(buf[4], buf[5]), 2, color, 2);
        Imgproc.circle(frame, new Point(buf[6], buf[7]), 2, color, 2);
        Imgproc.circle(frame, new Point(buf[8], buf[9]), 2, color, 2);
        Imgproc.circle(frame, new Point(buf[10], buf[11]), 2, color, 2);
        Imgproc.circle(frame, new Point(buf[12], buf[13]), 2, color, 2);
    }
}
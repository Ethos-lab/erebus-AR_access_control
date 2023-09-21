using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using HungarianAlgorithm;
using SortCS.Kalman;
using UnityEngine;

namespace SortCS
{
    public class SortTracker : ITracker
    {
        private readonly Dictionary<int, (Track Track, KalmanBoxTracker Tracker)> _trackers;
        private int _trackerIndex = 1; // MOT Evaluations requires a start index of 1

        public SortTracker(float iouThreshold = 0.3f, int maxMisses = 3)
        {
            _trackers = new Dictionary<int, (Track, KalmanBoxTracker)>();
            IouThreshold = iouThreshold;
            MaxMisses = maxMisses;
        }

        public float IouThreshold { get; private init; }

        public int MaxMisses { get; private init; }

        public IEnumerable<Track> Track(List<TrackData> trackData)
        {
            var predictions = new Dictionary<int, RectangleF>();

            foreach (var tracker in _trackers)
            {
                var prediction = tracker.Value.Tracker.Predict();
                predictions.Add(tracker.Key, prediction);
            }

            var (matchedTrackDataList, unmatchedTrackDataList) = MatchDetectionsWithPredictions(trackData, predictions.Values);

            var activeTrackids = new HashSet<int>();
            foreach (var item in matchedTrackDataList)
            {
                var prediction = predictions.ElementAt(item.Key);
                var track = _trackers[prediction.Key];
                if (track.Track.Label != item.Value.Label)
                {
                    track.Track.State = TrackState.Inactive;
                    _trackers.Remove(prediction.Key);
                    unmatchedTrackDataList.Add(item.Value);
                    continue;
                }
                track.Track.Misses = 0;
                track.Track.State = TrackState.Active;
                track.Tracker.Update(item.Value.Bbox);
                track.Track.RawScore = item.Value.RawScore;
                track.Track.Bbox = item.Value.Bbox;
                track.Track.Prediction = prediction.Value;

                activeTrackids.Add(track.Track.TrackId);
            }

            var missedTracks = _trackers.Where(x => !activeTrackids.Contains(x.Key));
            foreach (var missedTrack in missedTracks)
            {
                missedTrack.Value.Track.Misses++;
                missedTrack.Value.Track.TotalMisses++;
                missedTrack.Value.Track.State = TrackState.Missed;
            }

            var toRemove = _trackers.Where(x => x.Value.Track.Misses > MaxMisses).ToList();
            foreach (var tr in toRemove)
            {
                tr.Value.Track.State = TrackState.Inactive;
                _trackers.Remove(tr.Key);
            }

            foreach (var unmatchedTrackData in unmatchedTrackDataList)
            {
                var track = new Track
                {
                    TrackId = _trackerIndex++,
                    Misses = 0,
                    State = TrackState.New,
                    TotalMisses = 0,
                    Label = unmatchedTrackData.Label,
                    RawScore = unmatchedTrackData.RawScore,
                    Bbox = unmatchedTrackData.Bbox,
                    Prediction = unmatchedTrackData.Bbox
                };
                _trackers.Add(track.TrackId, (track, new KalmanBoxTracker(unmatchedTrackData.Bbox)));
            }

            //var allTracks = _trackers.Select(x => x.Value.Track).Concat(toRemove.Select(y => y.Value.Track));
            //return allTracks;
            var activeTracks = _trackers.Select(x => x.Value.Track);
            return activeTracks;
        }
        private (Dictionary<int, TrackData> Matched, List<TrackData> Unmatched) MatchDetectionsWithPredictions(
            List<TrackData> trackData,
            ICollection<RectangleF> trackPredictions)
        {
            if (trackPredictions.Count == 0)
            {
                return (new(), trackData);
            }

            var matrix = trackData.SelectMany((data) => trackPredictions.Select((trackPrediction) =>
            {
                var iou = IoU(data.Bbox, trackPrediction);

                return (int)(100 * -iou);
            })).ToArray(trackData.Count, trackPredictions.Count);

            if (trackData.Count > trackPredictions.Count)
            {
                var extra = new int[trackData.Count - trackPredictions.Count];
                matrix = Enumerable.Range(0, trackData.Count)
                    .SelectMany(row => Enumerable.Range(0, trackPredictions.Count).Select(col => matrix[row, col]).Concat(extra))
                    .ToArray(trackData.Count, trackData.Count);
            }

            var original = (int[,])matrix.Clone();
            var minimalThreshold = (int)(-IouThreshold * 100);
            var boxTrackerMapping = matrix.FindAssignments()
                .Select((ti, bi) => (bi, ti))
                .Where(bt => bt.ti < trackPredictions.Count && original[bt.bi, bt.ti] <= minimalThreshold)
                .ToDictionary(bt => bt.bi, bt => bt.ti);

            var unmatchedTrackDataArr = trackData.Where((_, index) => !boxTrackerMapping.ContainsKey(index)).ToList();
            var matchedTrackDataArr = trackData.Select((data, index) => boxTrackerMapping.TryGetValue(index, out var tracker)
                  ? (Tracker: tracker, TrackData: data)
                  : (Tracker: -1, TrackData: null))
               .Where(tb => tb.Tracker != -1)
               .ToDictionary(tb => tb.Tracker, tb => tb.TrackData);

            return (matchedTrackDataArr, unmatchedTrackDataArr);
        }
        private double IoU(RectangleF a, RectangleF b)
        {
            RectangleF intersection = RectangleF.Intersect(a, b);
            if (intersection.IsEmpty)
            {
                return 0;
            }

            double intersectArea = (1.0 + intersection.Width) * (1.0 + intersection.Height);
            double unionArea = ((1.0 + a.Width) * (1.0 + a.Height)) + ((1.0 + b.Width) * (1.0 + b.Height)) - intersectArea;
            return intersectArea / (unionArea + 1e-5);
        }

        //private void Log(IEnumerable<Track> tracks)
        //{
        //    if (!tracks.Any())
        //    {
        //        return;
        //    }

        //    var tracksWithHistory = tracks.Where(x => x.History != null);
        //    var longest = tracksWithHistory.Max(x => x.History.Count);
        //    var anyStarted = tracksWithHistory.Any(x => x.History.Count == 1 && x.Misses == 0);
        //    var ended = tracks.Count(x => x.State == TrackState.Ended);
        //    if (anyStarted || ended > 0)
        //    {
        //        var tracksStr = tracks.Select(x => $"{x.TrackId}{(x.State == TrackState.Active ? null : $": {x.State}")}");
        //    }
        //}
    }
}

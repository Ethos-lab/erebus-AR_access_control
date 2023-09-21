using System.Collections.Generic;
using System.Drawing;

namespace SortCS
{
    public record Track
    {
        public int TrackId { get; set; } = -1;
        public RectangleF Bbox { get; set; }
        public float[] RawScore { get; set; }
        public string Label { get; set; }

        public int TotalMisses { get; set; }

        public int Misses { get; set; }

        //public List<RectangleF> History { get; set; }

        public TrackState State { get; set; }

        public RectangleF Prediction { get; set; }
    }
    public class TrackData
    {
        public int TrackId { get; set; }
        public RectangleF Bbox { get; set; }
        public float[] RawScore { get; set; }
        public string Label { get; set; }
    }
}
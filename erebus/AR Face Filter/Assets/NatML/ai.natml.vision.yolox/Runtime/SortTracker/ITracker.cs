using System.Collections.Generic;
using System.Drawing;

namespace SortCS
{
    public interface ITracker
    {
        IEnumerable<Track> Track(List<TrackData> trackData);
    }
}
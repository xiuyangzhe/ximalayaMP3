using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ximalayaMP3
{
    public class AlbumData
    {
        public long AlbumId { get; set; }

        public long TrackTotalCount { get; set; }

        public List<TrackInfoDetail> Tracks { get; set; }

    }
}

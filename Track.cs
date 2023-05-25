using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ximalayaMP3
{
    public class Track
    {
        public TrackInfoDetail TrackInfo { get;set; }

    }

    public class TrackInfoDetail
    {
        public string Title { get; set; }

        public string No { get; set; }

        public string Src { get; set; }

        public long TrackId { get; set; }
    }
}

using System;
using System.Drawing;
using System.Xml.Linq;

namespace NWXNet
{
    public class ChartResponse : IResponseData
    {
        public string Id { get; set; }
        public Image Chart { get; set; }


        public int Width { get; set; }
        public int Height { get; set; }
        public int Level { get; set; }
        public DateTime Epoch { get; set; }
        public LatLon LowerLeftCoordinates { get; set; }
        public LatLon UpperRightCoordinates { get; set; }

        public DateTime Expires { get; set; }
    }
}
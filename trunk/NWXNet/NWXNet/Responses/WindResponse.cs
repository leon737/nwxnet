using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NWXNet
{
    public class WindResponse : IResponseData
    {
        public LatLon Coordinates { get; set; }
        public double Direction { get; set; }
        public double Speed { get; set; }
        public int Altitude { get; set; }
        public AltitudeUnit Unit { get; set; }
        public string Id { get; set; }
        public DateTime? Epoch { get; set; }

        public override string ToString()
        {
            return "[" + Coordinates + " @ " + Epoch + "] Winds @ " + NWXNet.Altitude.FormatUnitString(Altitude, Unit) + ": " +
                   Direction + " @ " + Speed + " kt";
         //   return "Winds at " + Coordinates + " - " + NWXNet.Altitude.FormatUnitString(Altitude, Unit) + ": " + Direction + " @ " + Speed + " kt";
        }
    }
}

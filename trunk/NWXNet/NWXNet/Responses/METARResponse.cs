using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NWXNet
{
    public class METARResponse : IResponseData
    {
        public string ICAO { get; set; }
        public LatLon Location { get; set; }
        public double DistanceFromRequestedLocation { get; set; }
        public readonly List<METARReport> Reports = new List<METARReport>();

        public METARReport this[string epoch]
        {
            get
            {
                return Reports.Where(x => x.Time == DateTime.Parse(epoch)).FirstOrDefault();
            }
        }

        public override string ToString()
        {
            var builder = new StringBuilder(String.Concat("[", Location, "] ", ICAO, (DistanceFromRequestedLocation == 0 ? ":" : " - " + DistanceFromRequestedLocation + "nm from request:"), Environment.NewLine));
            foreach (var metarReport in Reports)
            {
                builder.Append(metarReport);
                builder.Append(Environment.NewLine);
            }
            return builder.ToString();
        }
    }

    public class METARReport
    {
        public DateTime Time { get; set; }
        public string METAR { get; set; }

        public override string ToString()
        {
            return String.Concat(Time, " - ", METAR);
        }
    }
}
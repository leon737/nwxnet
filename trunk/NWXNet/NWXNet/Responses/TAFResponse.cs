using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NWXNet
{
    public class TAFResponse : IResponseData
    {
        public string ICAO { get; set; }
        public LatLon Location { get; set; }
        public double DistanceFromRequestedLocation { get; set; }
        public readonly List<TAFReport> Reports = new List<TAFReport>();

        public TAFReport this[string epoch]
        {
            get
            {
                return Reports.Where(x => x.Time == DateTime.Parse(epoch)).FirstOrDefault();
            }
        }

        public override string ToString()
        {
            var builder = new StringBuilder(String.Concat("[", Location, "] ", ICAO, (DistanceFromRequestedLocation == 0 ? ":" : " - " + DistanceFromRequestedLocation + "nm from request:"), Environment.NewLine));
            foreach (var TAFReport in Reports)
            {
                builder.Append(TAFReport);
                builder.Append(Environment.NewLine);
            }
            return builder.ToString();
        }
    }

    public class TAFReport
    {
        public DateTime Time { get; set; }
        public string TAF { get; set; }

        public override string ToString()
        {
            return String.Concat(Time, " - ", TAF);
        }
    }
}
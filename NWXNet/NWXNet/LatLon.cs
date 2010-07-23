using System;
using System.Text.RegularExpressions;

namespace NWXNet
{
    /// <summary>
    /// Represents a latitude/longitude pair.
    /// </summary>
    public class LatLon
    {
        #region Equality Operators

        public bool Equals(LatLon other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Latitude.Equals(Latitude) && other.Longitude.Equals(Longitude);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (LatLon)) return false;
            return Equals((LatLon) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Latitude.GetHashCode() * 397) ^ Longitude.GetHashCode();
            }
        }

        public static bool operator ==(LatLon left, LatLon right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(LatLon left, LatLon right)
        {
            return !Equals(left, right);
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="LatLon"/> class.
        /// </summary>
        /// <param name="lat">The latitude.</param>
        /// <param name="lon">The longitude.</param>
        public LatLon(double lat, double lon)
        {
            Latitude = lat;
            Longitude = lon;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LatLon"/> class.
        /// </summary>
        /// <param name="coords">The lat/lon coordinates in decimal form (xx.x,xx.x).</param>
        public LatLon(string coords)
        {
            Match m = Regex.Match(coords,
                                  @"^(?<lat>-?(\d|[0-8]\d|90)(\.\d+)?),(?<lon>-?(\d{1,2}|1[0-7]\d|180)(\.\d+)?)$");
            if (!m.Success)
                throw new ArgumentException("Invalid lat/lon coordinates.", "coords");

            Latitude = Double.Parse(m.Groups["lat"].Value);
            Longitude = Double.Parse(m.Groups["lon"].Value);
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance in DMS format.
        /// </returns>
        public override string ToString()
        {
            return String.Concat(DmsFormat(Latitude), (Latitude < 0 ? "S" : "N"), " ", DmsFormat(Longitude),
                                 (Longitude < 0 ? "W" : "E"));
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="format">The format - "DECIMAL" for regular decimal format, "P" for NWX format, "DMS" for degrees-minutes-seconds format.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance in the specified format.
        /// </returns>
        public string ToString(string format)
        {
            switch (format.ToUpper())
            {
                case "DECIMAL":
                    string lat = Latitude < 0 ? Math.Abs(Latitude) + "S" : Latitude + "N";
                    string lon = Longitude < 0 ? Math.Abs(Longitude) + "W" : Longitude + "E";
                    return lat + "," + lon;

                case "P":
                    return Latitude + "," + Longitude;

                case "DMS":
                    return ToString();
            }
            return null;
        }

        private static string DmsFormat(double input)
        {
            input = Math.Abs(input);
            var degrees = (int) input;
            double minutesFloat = 60 * (input - degrees);
            var minutes = (int) minutesFloat;
            double secondsFloat = 60 * (minutesFloat - minutes);
            var seconds = (int) secondsFloat;
            if (seconds == 60)
            {
                minutes++;
                seconds = 0;
            }
            if (minutes == 60)
            {
                degrees++;
                minutes = 0;
            }
            return degrees + "°" + minutes + "'" + seconds + "\"";
        }
    }
}
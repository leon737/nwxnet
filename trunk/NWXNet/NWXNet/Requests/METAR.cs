using System;

namespace NWXNet
{
    /// <summary>
    /// Represents a request for a METAR at a given ICAO or set of coordinates.
    /// </summary>
    public class METAR : IRequestData
    {
        internal string Icao { get; private set; }

        internal LatLon Coordinates { get; private set; }

        internal int Count { get; private set; }

        internal int MaxAge { get; private set; }

        private METAR(string icao)
        {
            Icao = icao;
        }

        private METAR(LatLon coords)
        {
            Coordinates = coords;
        }

        /// <summary>
        /// Create a METAR request for a specific ICAO.
        /// </summary>
        /// <param name="icao">The ICAO to retrieve METAR data for.</param>
        /// <returns>New METAR request object.</returns>
        public static METAR ForICAO(string icao)
        {
            return new METAR(icao);
        }

        /// <summary>
        /// Create a METAR request for a specific latitude/longitude.
        /// </summary>
        /// <param name="coords">The latitude/longitude, in decimal format (xx.x,xx.x).</param>
        /// <returns>New METAR request object.</returns>
        public static METAR ForCoordinates(string coords)
        {
            return ForCoordinates(new LatLon(coords));
        }

        /// <summary>
        /// Create a METAR request for a specific latitude/longitude.
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <returns>New METAR request object.</returns>
        public static METAR ForCoordinates(double latitude, double longitude)
        {
            return ForCoordinates(new LatLon(latitude, longitude));
        }

        /// <summary>
        /// Create a METAR request for a specific latitude/longitude.
        /// </summary>
        /// <param name="coords">A LatLon object specifying the coordinates.</param>
        /// <returns>New METAR request object.</returns>
        public static METAR ForCoordinates(LatLon coords)
        {
            return new METAR(coords);
        }

        /// <summary>
        /// Specifies that this request should retrieve the last specified number of reports.
        /// </summary>
        /// <param name="count">The number of reports to retrieve.</param>
        /// <returns></returns>
        public METAR Last(int count)
        {
            Count = count;
            return this;
        }

        /// <summary>
        /// Specified that this request should only retrieve reports that are no older than the specified number of hours.
        /// </summary>
        /// <param name="age">The maximum number of hours old that a retrieved report should be.</param>
        /// <returns></returns>
        public METAR WithMaxAge(int age)
        {
            MaxAge = age;
            return this;
        }

        #region Implementation of IRequestData

        RequestTypes IRequestData.Type
        {
            get { return RequestTypes.METAR; }
        }

        bool IRequestData.IsValid
        {
            get { return (Icao != null || Coordinates != null); }
        }

        public string Id
        {
            get { return Icao; }
        }

        #endregion
    }
}
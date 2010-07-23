using System;

namespace NWXNet
{
    /// <summary>
    /// Represents a request for a TAF at a given ICAO or set of coordinates.
    /// </summary>
    public class TAF : IRequestData
    {
        public string Icao { get; private set; }

        public LatLon Coordinates { get; private set; }

        public int Count { get; private set; }

        public int MaxAge { get; private set; }

        private TAF(string icao)
        {
            Icao = icao;
        }

        private TAF(LatLon coords)
        {
            Coordinates = coords;
        }

        /// <summary>
        /// Create a TAF request for a specific ICAO.
        /// </summary>
        /// <param name="icao">The ICAO to retrieve TAF data for.</param>
        /// <returns>New TAF request object.</returns>
        public static TAF ForICAO(string icao)
        {
            return new TAF(icao);
        }

        /// <summary>
        /// Create a TAF request for a specific latitude/longitude.
        /// </summary>
        /// <param name="coords">The latitude/longitude, in decimal format (xx.x,xx.x).</param>
        /// <returns>New TAF request object.</returns>
        public static TAF ForCoordinates(string coords)
        {
            return ForCoordinates(new LatLon(coords));
        }

        /// <summary>
        /// Create a TAF request for a specific latitude/longitude.
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <returns>New TAF request object.</returns>
        public static TAF ForCoordinates(double latitude, double longitude)
        {
            return ForCoordinates(new LatLon(latitude, longitude));
        }

        /// <summary>
        /// Create a TAF request for a specific latitude/longitude.
        /// </summary>
        /// <param name="coords">A LatLon object specifying the coordinates.</param>
        /// <returns>New TAF request object.</returns>
        public static TAF ForCoordinates(LatLon coords)
        {
            return new TAF(coords);
        }

        /// <summary>
        /// Specifies that this request should retrieve the last specified number of reports.
        /// </summary>
        /// <param name="count">The number of reports to retrieve.</param>
        /// <returns></returns>
        public TAF Last(int count)
        {
            Count = count;
            return this;
        }

        /// <summary>
        /// Specified that this request should only retrieve reports that are no older than the specified number of hours.
        /// </summary>
        /// <param name="age">The maximum number of hours old that a retrieved report should be.</param>
        /// <returns></returns>
        public TAF WithMaxAge(int age)
        {
            MaxAge = age;
            return this;
        }

        #region Implementation of IRequestData

        public RequestTypes Type
        {
            get { return RequestTypes.TAF; }
        }

        public bool IsValid
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
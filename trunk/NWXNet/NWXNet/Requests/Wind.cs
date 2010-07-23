using System;

namespace NWXNet
{
    /// <summary>
    /// Represents a request for wind data at a given set of coordinates, altitude and time.
    /// </summary>
    public class Wind : IRequestData
    {
        private Wind(LatLon coords)
        {
            Coordinates = coords;
        }

        public LatLon Coordinates { get; private set; }
        public int Altitude { get; private set; }
        public AltitudeUnit Unit { get; private set; }
        public DateTime Epoch { get; private set; }


        private string _id;
        public string Id
        {
            get { return _id ?? Coordinates.ToString("p"); }
            private set { _id = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Wind"/> class.
        /// </summary>
        /// <param name="coords">The coordinates of the request.</param>
        /// <param name="altitude">The altitude of the request.</param>
        /// <param name="unit">The unit for the altitude (Imperial/Metric Flight Level/Altitude).</param>
        /// <param name="epoch">The time to request data for.</param>
        private Wind(LatLon coords, int altitude, AltitudeUnit unit, DateTime epoch)
        {
            Coordinates = coords;
            Altitude = altitude;
            Unit = unit;
            Epoch = epoch;
        }

        /// <summary>
        /// Create a wind request for a specified set of coordinates, altitude and time.
        /// </summary>
        /// <param name="coords">The coordinates of the request.</param>
        /// <param name="altitude">The altitude of the request.</param>
        /// <param name="unit">The unit for the altitude (Imperial/Metric Flight Level/Altitude).</param>
        /// <param name="epoch">The time to request data for.</param>
        /// <returns></returns>
        public static Wind For(string coords, int altitude, AltitudeUnit unit, DateTime epoch)
        {
            return new Wind(new LatLon(coords), altitude, unit, epoch);
        }

        /// <summary>
        /// Specifies that this request should use the given identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public Wind WithId(string id)
        {
            Id = id;
            return this;
        }

        #region Implementation of IRequestData

        public RequestTypes Type
        {
            get { return RequestTypes.Wind; }
        }

        public bool IsValid
        {
            get { return (Coordinates != null) && (Altitude != 0) && (Unit != AltitudeUnit.None) && (Epoch != DateTime.MinValue); }
        }

        #endregion
    }
}
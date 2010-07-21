using System;

namespace NWXNet
{
    public class Wind : IRequestData
    {
        public LatLon Coordinates { get; private set; }
        public int Altitude { get; private set; }
        public AltitudeUnit Unit { get; private set; }
        public DateTime Epoch { get; private set; }

        public string Id { get; private set; }

        private Wind(LatLon coords, int altitude, AltitudeUnit unit, DateTime epoch)
        {
            Coordinates = coords;
            Altitude = altitude;
            Unit = unit;
            Epoch = epoch;
        }

        public static Wind At(string coords, int altitude, AltitudeUnit unit, DateTime epoch)
        {
            return new Wind(new LatLon(coords), altitude, unit, epoch);
        }

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

        #endregion
    }
}
namespace NWXNet
{
    public class METAR : IRequestData
    {
        private string _icao;
        public string Icao
        {
            get { return _icao; }
        }

        private LatLon _coordinates;
        public LatLon Coordinates
        {
            get { return _coordinates; }
        }

        private int _count;
        public int Count
        {
            get { return _count; }
        }

        private int _maxage;
        public int MaxAge
        {
            get { return _maxage; }
        }

        private METAR(string icao)
        {
            _icao = icao;
        }

        private METAR(LatLon coords)
        {
            _coordinates = coords;
        }

        public static METAR ForICAO(string icao)
        {
            return new METAR(icao);
        }
        
        public static METAR ForCoordinates(string coords)
        {
            return new METAR(new LatLon(coords));
        }

        public METAR Last(int count)
        {
            _count = count;
            return this;
        }

        public METAR WithMaxAge(int age)
        {
            _maxage = age;
            return this;
        }

        #region Implementation of IRequestData

        public RequestTypes Type
        {
            get { return RequestTypes.METAR; }
        }

        #endregion
    }
}
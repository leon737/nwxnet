namespace NWXNet
{
    public class GeoMagModels : IRequestData
    {
        public RequestTypes Type
        {
            get { return RequestTypes.AvailableGeoMagModels; }
        }

        public bool IsValid
        {
            get { return true; }
        }

        public string Id
        {
            get { return "AvailableGeoMagModels"; }
        }
    }
}
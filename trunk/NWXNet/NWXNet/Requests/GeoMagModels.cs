namespace NWXNet
{
    public class GeoMagModels : IRequestData
    {
        RequestTypes IRequestData.Type
        {
            get { return RequestTypes.AvailableGeoMagModels; }
        }

        bool IRequestData.IsValid
        {
            get { return true; }
        }

        public string Id
        {
            get { return "AvailableGeoMagModels"; }
        }
    }
}
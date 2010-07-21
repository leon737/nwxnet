namespace NWXNet
{
    public class Levels : IRequestData
    {
        #region Implementation of IRequestData

        public RequestTypes Type
        {
            get { return RequestTypes.AvailableLevels; }
        }

        #endregion
    }
}

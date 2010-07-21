namespace NWXNet
{
    public class Epochs : IRequestData
    {
        #region Implementation of IRequestData

        public RequestTypes Type
        {
            get { return RequestTypes.AvailableEpochs; }
        }

        #endregion
    }
}
using System;

namespace NWXNet
{
    public class Epochs : IRequestData
    {
        #region Implementation of IRequestData

        public RequestTypes Type
        {
            get { return RequestTypes.AvailableEpochs; }
        }

        public bool IsValid
        {
            get { return true; }
        }

        public string Id
        {
            get { return "AvailableEpochs"; }
        }

        #endregion
    }
}
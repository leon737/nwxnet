using System;

namespace NWXNet
{
    public class Epochs : IRequestData
    {
        #region Implementation of IRequestData

        RequestTypes IRequestData.Type
        {
            get { return RequestTypes.AvailableEpochs; }
        }

        bool IRequestData.IsValid
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
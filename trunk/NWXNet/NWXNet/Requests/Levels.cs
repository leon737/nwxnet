using System;

namespace NWXNet
{
    public class Levels : IRequestData
    {
        #region Implementation of IRequestData

        RequestTypes IRequestData.Type
        {
            get { return RequestTypes.AvailableLevels; }
        }

        bool IRequestData.IsValid
        {
            get { return true; }
        }

        public string Id
        {
            get { return "AvailableLevels"; }
        }

        #endregion
    }
}

using System;

namespace NWXNet
{
    public class Levels : IRequestData
    {
        #region Implementation of IRequestData

        public RequestTypes Type
        {
            get { return RequestTypes.AvailableLevels; }
        }

        public bool IsValid
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

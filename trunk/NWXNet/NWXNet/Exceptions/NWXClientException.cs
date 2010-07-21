using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NWXNet.Exceptions
{
    [Serializable]
    public class NWXClientException : Exception
    {

        public NWXClientException()
            : base()
        {
            
        }

        public NWXClientException(string message)
            : base(message)
        {
            
        }

        public NWXClientException(string message, Exception inner)
            : base(message, inner)
        {
            
        }
    }
}

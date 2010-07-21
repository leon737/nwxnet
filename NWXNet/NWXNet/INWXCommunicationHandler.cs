using System;
using System.Collections.Generic;
using System.Linq;

namespace NWXNet
{
    public interface INWXCommunicationHandler
    {
        string Send(string data);
    }
}

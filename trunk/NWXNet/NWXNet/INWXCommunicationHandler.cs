using System;
using System.Collections.Generic;
using System.Linq;

namespace NWXNet
{
    public interface INWXCommunicationHandler
    {
        /// <summary>
        /// Sends the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>String data response.</returns>
        string Send(string data);
    }
}

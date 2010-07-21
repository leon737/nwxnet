using System.IO;
using System.Net;
using System.Text;

namespace NWXNet
{
    public class HttpCommunicationHandler : INWXCommunicationHandler
    {
        #region Implementation of INWXCommunicationHandler

        public string Send(string data)
        {
            WebRequest request = WebRequest.Create("http://navlost.eu/aero/nwx");
            request.Method = "POST";
            request.ContentType = "text/xml";

            byte[] bytes = Encoding.ASCII.GetBytes(data);
            Stream os = null;
            try
            {
                request.ContentLength = bytes.Length;
                os = request.GetRequestStream();
                os.Write(bytes, 0, bytes.Length);
            }
            finally
            {
                if(os != null)
                    os.Close();
            }

            WebResponse response = request.GetResponse();
            if (response == null)
                return null;
            StreamReader sr = new StreamReader(response.GetResponseStream());
            return sr.ReadToEnd().Trim();
        }

        #endregion
    }
}
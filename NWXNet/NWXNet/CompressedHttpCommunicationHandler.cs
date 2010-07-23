using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;

namespace NWXNet
{
    public class CompressedHttpCommunicationHandler : INWXCommunicationHandler
    {
        public string Send(string data)
        {
            WebRequest request = WebRequest.Create("http://navlost.eu/aero/nwx");
            request.Method = "POST";
            request.ContentType = "text/xml";
            request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip");

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
                if (os != null)
                    os.Close();
            }

            var response = request.GetResponse() as HttpWebResponse;
            if (response == null)
                return null;

            Stream responseStream = response.GetResponseStream();
            if (response.ContentEncoding.ToLower().Contains("gzip"))
            {
                responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
            }
            var sr = new StreamReader(responseStream, Encoding.Default);
            return sr.ReadToEnd().Trim();
        }
    }
}
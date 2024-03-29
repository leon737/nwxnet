﻿using System.IO;
using System.IO.Compression;
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

            var response = request.GetResponse() as HttpWebResponse;
            if (response == null)
                return null;

            Stream responseStream = response.GetResponseStream();
            var sr = new StreamReader(responseStream, Encoding.Default);
            return sr.ReadToEnd().Trim();
        }

        #endregion
    }

}
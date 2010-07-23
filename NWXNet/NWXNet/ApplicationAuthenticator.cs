using System;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace NWXNet
{
    public class ApplicationAuthenticator : INWXAuthenticator
    {
        private readonly string _name;
        private readonly string _secret;
        private readonly int _instance;
        private DateTime _lastUtc;
        private string _lastToken;

        private TimeSpan? _timeout;
        public TimeSpan? Timeout
        {
            get { return _timeout ?? (_timeout = TimeSpan.FromSeconds(3500)); }
            set { _timeout = value; }
        }

        public ApplicationAuthenticator(string name, int instance, string secret)
        {
            _name = name;
            _instance = instance;
            _secret = secret;
        }

        private string GenerateToken()
        {
            if (DateTime.UtcNow.Subtract(_lastUtc) > Timeout)
            {
                _lastUtc = DateTime.UtcNow;
                string data = string.Concat(_name, _instance, _secret, _lastUtc.ToNWXString());

                byte[] buffer = Encoding.ASCII.GetBytes(data);
                var sha1 = new SHA1CryptoServiceProvider();
                _lastToken = BitConverter.ToString(sha1.ComputeHash(buffer)).Replace("-", "").ToLower();
            }
            return _lastToken;
        }

        public XElement GenerateXml()
        {
            return new XElement("application",
                                new XAttribute("name", _name),
                                new XAttribute("instance", _instance),
                                new XAttribute("token", GenerateToken()),
                                new XAttribute("timestamp", _lastUtc.ToNWXString()));
        }
    }
}
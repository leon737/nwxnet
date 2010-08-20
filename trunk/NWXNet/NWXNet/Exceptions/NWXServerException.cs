using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NWXNet.Exceptions
{
    [Serializable]
    public class NWXServerException : Exception
    {
        private List<ServerError> _errors;
        public List<ServerError> Errors
        {
            get { return _errors ?? (_errors = new List<ServerError>()); }
            set { _errors = value; }
        }

        public string RawResponseXml { get; set; }

        public NWXServerException()
            : base()
        {

        }

        public NWXServerException(string message)
        {
            Errors.Add(new ServerError(message));
        }

        public NWXServerException(params ServerError[] errors)
        {
            Errors = new List<ServerError>(errors);
        }

        public NWXServerException(List<ServerError> errors)
        {
            Errors = errors;
        }

        public NWXServerException(string message, Exception inner)
            : base(message, inner)
        {
            Errors.Add(new ServerError(message));
        }

        public override string Message
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var serverError in Errors)
                {
                    if (!string.IsNullOrEmpty(serverError.ErrorCode))
                    {
                        sb.Append("Error code: ");
                        sb.AppendLine(serverError.ErrorCode);
                    }
                    sb.Append(serverError.Message);
                }
                return sb.ToString();
            }
        }
    }

    public class ServerError
    {
        public string ErrorCode { get; set; }
        public string Message { get; set; }


        public ServerError()
        {
            
        }

        public ServerError(string message)
        {
            Message = message;
        }

        public ServerError(string errorCode, string message)
        {
            ErrorCode = errorCode;
            Message = message;
        }

        public bool Equals(ServerError other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.ErrorCode, ErrorCode) && Equals(other.Message, Message);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ServerError)) return false;
            return Equals((ServerError) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((ErrorCode != null ? ErrorCode.GetHashCode() : 0) * 397) ^ (Message != null ? Message.GetHashCode() : 0);
            }
        }
    }
}


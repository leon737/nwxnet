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

        }

        public override string Message
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var serverError in Errors)
                {
                    if (serverError.ErrorCode != 0)
                    {
                        sb.Append("Error code: ");
                        sb.AppendLine(serverError.ErrorCode.ToString());
                    }
                    sb.Append(serverError.Message);
                }
                return sb.ToString();
            }
        }
    }

    public class ServerError
    {
        public int ErrorCode { get; set; }
        public string Message { get; set; }


        public ServerError()
        {
            
        }

        public ServerError(string message)
        {
            Message = message;
        }

        public ServerError(int errorCode, string message)
        {
            ErrorCode = errorCode;
            Message = message;
        }

        public override bool Equals(object obj)
        {
            var compareTo = obj as ServerError;
            return compareTo == null ? false : (ErrorCode == compareTo.ErrorCode) && (Message.Equals(compareTo.Message));
        }

        public bool Equals(ServerError compareTo)
        {
            return compareTo == null ? false : (ErrorCode == compareTo.ErrorCode) && (Message.Equals(compareTo.Message));
        }

        public override int GetHashCode()
        {
            return ErrorCode ^ Message.GetHashCode();
        }
    }
}


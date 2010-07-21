using System;
using System.Collections.Generic;
using NWXNet.Exceptions;

namespace NWXNet
{
    /// <summary>
    /// Represents a single request to NWX.  May contain many separate data requests.
    /// </summary>
    public class Request
    {
        private readonly INWXCommunicationHandler _communicationHandler;
        private readonly INWXSerializer _serializer;
        private List<IRequestData> _data = new List<IRequestData>();
        private Version _version = new Version("0.3.5");

        /// <summary>
        /// Initializes a new instance of the <see cref="Request"/> class.  For internal use.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        /// <param name="handler">The handler.</param>
        public Request(INWXSerializer serializer, INWXCommunicationHandler handler)
        {
            _serializer = serializer;
            _communicationHandler = handler;
        }

        /// <summary>
        /// Gets or sets the version of the request - by default 0.3.5.
        /// </summary>
        /// <value>The version.</value>
        public Version Version
        {
            get { return _version; }
            set { _version = value; }
        }

        /// <summary>
        /// The actual data being requested.
        /// </summary>
        /// <value>The data.</value>
        public List<IRequestData> Data
        {
            get { return _data; }
            set { _data = value; }
        }

        /// <summary>
        /// Specify the version of the request.  User is responsible for not using requests that are not compatible with the specified version.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        public Request UsingVersion(string version) // TODO: Limit request types based on version.
        {
            Version = new Version(version);
            return this;
        }

        /// <summary>
        /// Specifies the types of data to be included in the request.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public Request For(params IRequestData[] data)
        {
            _data.AddRange(data);

            return this;
        }

        private string SerializeRequest()
        {
            try
            {
                return _serializer.Serialize(this);
            }
            catch (Exception ex)
            {
                throw new NWXClientException("Error during serialization of request.", ex);
            }
        }

        private string SendRequest(string rawXml)
        {
            try
            {
                return _communicationHandler.Send(rawXml);
            }
            catch (Exception ex)
            {
                throw new NWXClientException("Error during send/receive operation for request.", ex);
            }
        }

        private Response DeserializeResponse(string response)
        {
            try
            {
                return _serializer.Deserialize(response);
            }
            catch (Exception ex)
            {
                if (ex.GetType() != typeof (NWXServerException))
                    throw new NWXClientException("Error during deserialization of response.", ex);
                throw;
            }
        }

        /// <summary>
        /// Sends this request.
        /// </summary>
        /// <returns>Response from request.</returns>
        /// <exception cref="NWXClientException">When no data is provided or a client-side error occurs.</exception>
        /// <exception cref="NWXServerException">If the server does not return data properly or a communication error occurs.</exception>
        public Response Send()
        {
            if (_data.Count < 1 || _data.Count < 1)
                throw new NWXClientException("No data has been provided for the request.");
            string rawXml = SerializeRequest();
            string responseString = SendRequest(rawXml);
            return DeserializeResponse(responseString);
        }
    }
}
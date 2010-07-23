namespace NWXNet
{
    /// <summary>
    /// Base static class for initializing NWX requests.  Also contains global settings for new requests.
    /// </summary>
    public static class NWX
    {
        private static INWXSerializer _serializer = new NWXXmlSerializer();

        private static INWXCommunicationHandler _communicationHandler = new CompressedHttpCommunicationHandler();

        private static INWXAuthenticator _authenticator;

        /// <summary>
        /// Authentication method to use - by default, no authentication is used.  Choose from ApplicationAuthenticator or (future) UserAuthenticator.
        /// </summary>
        /// <value>The authenticator.</value>
        public static INWXAuthenticator Authenticator
        {
            set { _authenticator = value; }
        }

        /// <summary>
        /// Initialize a new Request object.
        /// </summary>
        /// <value>The request.</value>
        public static Request Request
        {
            get { return new Request(_serializer, _communicationHandler, _authenticator); }
        }

        /// <summary>
        /// Serializer to use - by default uses NWXXmlSerializer.  There should be no need to change this.
        /// </summary>
        /// <value>The serializer.</value>
        public static INWXSerializer Serializer
        {
            //get { return _serializer; }
            set { _serializer = value; }
        }

        /// <summary>
        /// Communication handler to use - by default HttpCommunicationHandler.  There should be no need to change this.
        /// </summary>
        /// <value>The communication handler.</value>
        public static INWXCommunicationHandler CommunicationHandler
        {
            //get { return _communicationHandler; }
            set { _communicationHandler = value; }
        }
    }
}
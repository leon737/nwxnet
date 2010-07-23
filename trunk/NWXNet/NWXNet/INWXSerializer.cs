using System.Text;

namespace NWXNet
{
    public interface INWXSerializer
    {
        /// <summary>
        /// Serializes the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>Serialized string of request.</returns>
        string Serialize(Request request);

        /// <summary>
        /// Deserializes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>Response object populated with data from initial data string.</returns>
        Response Deserialize(string data);
    }
}

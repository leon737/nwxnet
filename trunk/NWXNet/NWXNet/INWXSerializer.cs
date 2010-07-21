using System.Text;

namespace NWXNet
{
    public interface INWXSerializer
    {
        string Serialize(Request request);
        Response Deserialize(string data);
    }
}

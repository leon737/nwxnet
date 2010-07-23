using System.Xml.Linq;

namespace NWXNet
{
    public interface INWXAuthenticator
    {
        XElement GenerateXml();
    }
}
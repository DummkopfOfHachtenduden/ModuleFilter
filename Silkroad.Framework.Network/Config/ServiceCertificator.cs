using System.Xml;

namespace Silkroad.Framework.Common.Config
{
    public class ServiceCertificator
    {
        public ServiceCertificator(XmlNode node)
        {
            this.IP = node.Attributes[nameof(this.IP)].Value;
            this.Port = ushort.Parse(node.Attributes[nameof(this.Port)].Value);
        }

        public string IP { get; private set; }
        public ushort Port { get; private set; }
    }
}
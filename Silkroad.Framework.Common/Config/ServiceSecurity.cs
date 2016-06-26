using System.Xml;

namespace Silkroad.Framework.Common.Config
{
    public class ServiceSecurity
    {
        public ServiceSecurity(XmlNode node)
        {
            this.Blowfish = bool.Parse(node.Attributes[nameof(this.Blowfish)].Value);
            this.CRC = bool.Parse(node.Attributes[nameof(this.CRC)].Value);
            this.Handshake = bool.Parse(node.Attributes[nameof(this.Handshake)].Value);
        }

        public bool Blowfish { get; private set; }
        public bool CRC { get; private set; }
        public bool Handshake { get; private set; }
    }
}
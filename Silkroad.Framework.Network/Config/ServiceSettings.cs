using System;
using System.Xml;

namespace Silkroad.Framework.Common.Config
{
    public class ServiceSettings
    {
        public ServiceSettings(XmlNode node)
        {
            this.Name = node.Attributes["Name"].Value;
            this.Type = (ServiceType)Enum.Parse(typeof(ServiceType), node.Attributes["Type"].Value);
            this.IP = node.Attributes["IP"].Value;
            this.Port = ushort.Parse(node.Attributes["Port"].Value);

            var securityNode = node["Security"];
            this.Blowfish = bool.Parse(securityNode.Attributes[nameof(this.Blowfish)].Value);
            this.SecurityBytes = bool.Parse(securityNode.Attributes[nameof(this.SecurityBytes)].Value);
            this.Handshake = bool.Parse(securityNode.Attributes[nameof(this.Handshake)].Value);

            var certificatorNode = node["Certificator"];
            this.CertificatorIP = certificatorNode.Attributes["IP"].Value;
            this.CertificatorPort = ushort.Parse(certificatorNode.Attributes["Port"].Value);
        }

        public string Name { get; internal set; }
        public ServiceType Type { get; set; }
        public string IP { get; set; }
        public ushort Port { get; set; }

        public bool Blowfish { get; set; }
        public bool SecurityBytes { get; set; }
        public bool Handshake { get; set; }

        public string CertificatorIP { get; set; }
        public ushort CertificatorPort { get; set; }
    }
}
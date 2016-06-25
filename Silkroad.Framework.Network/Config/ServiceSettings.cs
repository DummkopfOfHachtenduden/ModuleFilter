using System;
using System.Collections.Generic;
using System.Xml;

namespace Silkroad.Framework.Common.Config
{
    public class ServiceSettings
    {
        public string Name { get; private set; }
        public ServiceType Type { get; private set; }
        public string IP { get; private set; }
        public ushort Port { get; private set; }

        public ServiceSecurity Security { get; private set; }
        public ServiceCertificator Certificator { get; private set; }

        private Dictionary<uint, ServiceRedirect> _redirections;
        public IReadOnlyDictionary<uint, ServiceRedirect> Redirections { get { return _redirections; } }

        public ServiceSettings(XmlNode node)
        {
            this.Name = node.Attributes["Name"].Value;
            this.Type = (ServiceType)Enum.Parse(typeof(ServiceType), node.Attributes[nameof(this.Type)].Value);
            this.IP = node.Attributes[nameof(this.IP)].Value;
            this.Port = ushort.Parse(node.Attributes[nameof(this.Port)].Value);

            this.Security = new ServiceSecurity(node["Security"]);
            this.Certificator = new ServiceCertificator(node["Certificator"]);

            _redirections = new Dictionary<uint, ServiceRedirect>();
            var redirectionNode = node["redirections"];
            if (redirectionNode == null)
                return;

            foreach (XmlNode redirectNode in redirectionNode)
            {
                if (redirectNode.NodeType != XmlNodeType.Element)
                    continue;

                var redirect = new ServiceRedirect(redirectNode);
                _redirections.Add(redirect.CoordID, redirect);
            }
        }
    }
}
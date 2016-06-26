using NLog;
using Silkroad.Framework.Common;
using Silkroad.Framework.Common.Config;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Silkroad.Tools.ModuleFilter.Config
{
    public class FilterConfig
    {
        public Dictionary<string, LogLevel> Logger { get; private set; }
        public Dictionary<string, ServiceSettings> Services { get; private set; }
        public Dictionary<string, ServiceType> Plugins { get; private set; }

        public FilterConfig(string fileName)
        {
            this.Logger = new Dictionary<string, LogLevel>();
            this.Services = new Dictionary<string, ServiceSettings>();
            this.Plugins = new Dictionary<string, ServiceType>();

            var xml = new XmlDocument();
            try
            {
                xml.Load(fileName);
                var root = xml["filter"];
                foreach (XmlNode node in root)
                {
                    if (node.NodeType != XmlNodeType.Element)
                        continue;

                    var name = node.Name.ToLowerInvariant();
                    switch (name)
                    {
                        case "logger":
                            this.ParseLogger(node);
                            break;

                        case "plugin":
                            this.ParsePlugin(node);
                            break;

                        case "service":
                            this.ParseService(node);
                            break;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void ParseLogger(XmlNode node)
        {
            var name = node.Attributes["Name"].Value;
            var logLevel = LogLevel.FromString(node.Attributes["LogLevel"].Value);
            Logger.Add(name, logLevel);
        }

        private void ParseService(XmlNode node)
        {
            var name = node.Attributes["Name"].Value;
            var settings = new ServiceSettings(node);
            Services.Add(name, settings);
        }

        private void ParsePlugin(XmlNode node)
        {
            var name = node.Attributes["Name"].Value;
            var serviceType = (ServiceType)Enum.Parse(typeof(ServiceType), node.Attributes["ServiceType"].Value);
            Plugins.Add(name, serviceType);
        }
    }
}
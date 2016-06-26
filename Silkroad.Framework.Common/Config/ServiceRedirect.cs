using System.Xml;

namespace Silkroad.Framework.Common.Config
{
    public class ServiceRedirect
    {
        public ServiceRedirect(XmlNode node)
        {
            this.CoordID = uint.Parse(node.Attributes[nameof(this.CoordID)].Value);
            this.MachineID = uint.Parse(node.Attributes[nameof(this.MachineID)].Value);
            this.Port = ushort.Parse(node.Attributes[nameof(this.Port)].Value);
        }

        public uint CoordID { get; private set; }
        public uint MachineID { get; private set; }
        public ushort Port { get; private set; }
    }
}
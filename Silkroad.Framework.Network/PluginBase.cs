using Silkroad.Framework.Common.Objects;
using Silkroad.Framework.Common.Security;
using Silkroad.Framework.Utility;

namespace Silkroad.Framework.Common
{
    public class PluginBase : IPlugin
    {
        #region Fields

        private CertifiactionManager _certificationManager;

        #endregion Fields

        #region Properties

        public string Name { get; private set; }

        public Service Service { get; private set; }

        #endregion Properties

        public PluginBase()
        {
            _certificationManager = new CertifiactionManager();
        }

        #region Methods

        public virtual void Register(string name, Service service)
        {
            this.Name = name;
            this.Service = service;

            this.Service.PacketManager.AddModuleHandler(0x6003, CertificationReq);
            this.Service.PacketManager.AddCertificatorHandler(0xA003, CertificationAck);

            //Add packet handlers used in all plugins here...
        }

        private PacketResult CertificationReq(Session arg1, Packet arg2)
        {
            _certificationManager.ReadReq(arg2);

            //Additional spoofing possability here...

            return PacketResult.None;
        }

        private PacketResult CertificationAck(Session arg1, Packet arg2)
        {
            var result = new PacketResult(PacketResultAction.Replace);

            _certificationManager.ReadAck(arg2);

            foreach (var redirect in this.Service.Settings.Redirections)
            {
                if (_certificationManager.NodeLinks.ContainsKey(redirect.CoordID))
                {
                    var link = _certificationManager.NodeLinks[redirect.CoordID];

                    var parentNode = _certificationManager.NodeData[link.ParentNodeID];

                    //SPOOF
                    //parentNode.NodeType = redirect.MachineID;
                    parentNode.Port = redirect.Port;

                    _certificationManager.NodeData[link.ParentNodeID] = parentNode;
                }
                else
                {
                    StaticLogger.Logger[this.Name].Fatal($"Coord({redirect.CoordID}) not found. Redirect impossible, please check Filter.xml!");
                }
            }

            var packet = new Packet(arg2.Opcode, arg2.Encrypted, arg2.Massive);
            _certificationManager.WriteAck(packet, true, true);

            result.Add(packet);

            return result;
        }

        #endregion Methods
    }
}
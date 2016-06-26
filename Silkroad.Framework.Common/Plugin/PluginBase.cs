using Silkroad.Framework.Common.Objects;
using Silkroad.Framework.Common.Security;
using Silkroad.Framework.Utility;

namespace Silkroad.Framework.Common.Plugin
{
    public class PluginBase : IPlugin
    {
        #region Properties

        public int Index { get; protected set; }

        public string Name { get; protected set; }

        public Service Service { get; protected set; }

        public CertifiactionManager CertificationManager { get; protected set; }

        public IPluginControl Control { get; protected set; }

        #endregion Properties

        #region Methods (public virtual)

        public virtual void Register(string name, Service service)
        {
            this.Name = name;
            this.Service = service;

            this.CertificationManager = new CertifiactionManager();
            this.Service.PacketManager.AddModuleHandler(0x6003, CertificationReq);
            this.Service.PacketManager.AddCertificatorHandler(0xA003, CertificationAck);
            //Add packet handlers used in all plugins here...

            //0x6008,
            //0xA008,
        }

        public virtual void InitializeUI()
        {
            this.Control = new PluginControl(this);
        }

        #endregion Methods (public virtual)

        #region Methods (protected virtual)

        protected virtual PacketResult CertificationReq(Session arg1, Packet arg2)
        {
            var result = new PacketResult(PacketResultAction.Replace);
            var response = new Packet(arg2.Opcode, arg2.Encrypted, arg2.Massive);

            CertificationManager.ReadReq(arg2);

            //_certificationManager.RequestIP = "192.168.178.10";

            CertificationManager.WriteReq(response);

            result.Add(response);
            return result;
        }

        protected virtual PacketResult CertificationAck(Session arg1, Packet arg2)
        {
            var result = new PacketResult(PacketResultAction.Replace);

            CertificationManager.ReadAck(arg2);

            foreach (var redirect in this.Service.Settings.Redirections)
            {
                if (CertificationManager.NodeLinks.ContainsKey(redirect.CoordID))
                {
                    var link = CertificationManager.NodeLinks[redirect.CoordID];

                    var parentNode = CertificationManager.NodeData[link.ParentNodeID];

                    //SPOOF
                    //parentNode.NodeType = redirect.MachineID;
                    parentNode.Port = redirect.Port;

                    CertificationManager.NodeData[link.ParentNodeID] = parentNode;
                }
                else
                {
                    StaticLogger.Logger[this.Name].Fatal($"Coord({redirect.CoordID}) not found. Redirect impossible, please check Filter.xml!");
                }
            }

            var packet = new Packet(arg2.Opcode, arg2.Encrypted, arg2.Massive);
            CertificationManager.WriteAck(packet, true, true);

            result.Add(packet);

            return result;
        }

        #endregion Methods (protected virtual)
    }
}
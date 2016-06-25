using Silkroad.Framework.Common.Objects;
using Silkroad.Framework.Common.Security;

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

            for (int i = 0; i < _certificationManager.NodeData.Count; i++)
            {
                var data = _certificationManager.NodeData[i];

                //data.NodeType = 336;
                data.Port = 20001;

                _certificationManager.NodeData[i] = data;
            }

            var packet = new Packet(arg2.Opcode, arg2.Encrypted, arg2.Massive);
            _certificationManager.WriteAck(packet, true, true);

            result.Add(packet);

            return result;
        }

        #endregion Methods
    }
}
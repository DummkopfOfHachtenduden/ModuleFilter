using Silkroad.Framework.Common;
using Silkroad.Framework.Common.Security;
using Silkroad.Framework.Utility;

namespace Silkroad.Plugin.Gateway
{
    public class Plugin : IPlugin
    {
        private Service _service;

        public void Register(Service service)
        {
            _service = service;

            //service.PacketManager.AddModuleHandler(0x6003, On6003);
            service.PacketManager.AddCertificatorHandler(0xA003, OnA003);
        }

        private PacketResult On6003(Session session, Packet packet)
        {
            var name = packet.ReadAscii();
            var ip = packet.ReadAscii();

            StaticLogger.Instance.Info($"{Caller.GetMemberName()}: {name} [{ip}]");

            return PacketResult.None;
        }

        private PacketResult OnA003(Session session, Packet packet)
        {
            var result = new PacketResult(PacketResultAction.Replace);

            _service.CertificationManager.Read(packet);

            //TODO: Make changes to certification
            for (int i = 0; i < _service.CertificationManager.NodeData.Count; i++)
            {
                var nodeData = _service.CertificationManager.NodeData[i];

                if (nodeData.NodeID == 697)
                    nodeData.Port = 20000;

                _service.CertificationManager.NodeData[i] = nodeData;
            }

            var spoofPacket = new Packet(packet.Opcode, packet.Encrypted, packet.Massive);
            _service.CertificationManager.Write(spoofPacket, true, true);

            result.Add(spoofPacket);

            return result;
        }
    }
}
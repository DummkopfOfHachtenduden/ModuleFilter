using Silkroad.Framework.Common;
using Silkroad.Framework.Common.Security;
using System;
using System.Text;

namespace Silkroad.Plugin.Gateway
{
    public class Plugin : IPlugin
    {
        private Service _service;

        public void Register(Service service)
        {
            _service = service;

            service.PacketManager.AddCertificatorHandler(0xA003, OnA003);
        }

        private PacketResult OnA003(Session session, Packet packet)
        {
            //TODO: spoof ips and ports propperly

            var payload = packet.GetBytes();

            var srcIP = Encoding.ASCII.GetBytes(_service.Settings.CertificatorIP);
            var destIP = Encoding.ASCII.GetBytes(_service.Settings.IP);

            var spoof1 = payload.Replace(srcIP, destIP);

            var certifiactorPort = BitConverter.GetBytes(_service.Settings.CertificatorPort);
            var destPort = BitConverter.GetBytes(_service.Settings.Port);

            var spoof2 = spoof1.Replace(certifiactorPort, destPort);

            var spoofPacket = new Packet(packet.Opcode, false, true, spoof2);

            var result = new PacketResult(PacketResultAction.Replace);
            result.Add(spoofPacket);

            return result;

            //return PacketResult.None;
        }
    }
}
using Silkroad.Framework.Common;

namespace Silkroad.Plugin.Gateway
{
    public class Plugin : IPlugin
    {
        private Service _service;

        public void Register(Service service)
        {
            _service = service;

            //service.PacketManager.AddModuleHandler(0x6003, On6003);
            //service.PacketManager.AddCertificatorHandler(0xA003, OnA003);
        }
    }
}
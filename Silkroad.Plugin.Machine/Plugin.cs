using Silkroad.Framework.Common;
using Silkroad.Framework.Common.Plugin;

namespace Silkroad.Plugin.Machine
{
    public class Plugin : PluginBase
    {
        public override void Register(string name, Service service)
        {
            base.Register(name, service);

            //Add plugin related packet handlers here...
        }
    }
}
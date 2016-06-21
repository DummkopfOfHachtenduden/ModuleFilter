using Silkroad.Framework.Common.Config;
using Silkroad.Framework.Utility;

namespace Silkroad.Framework.Common
{
    public class Service
    {
        public ServiceSettings Settings { get; private set; }

        public ServiceListener ServiceListener { get; private set; }
        public SessionPool SessionPool { get; private set; }
        public SessionManager SessionManager { get; private set; }
        public PacketManager PacketManager { get; private set; }

        public Service(ServiceSettings settings)
        {
            this.Settings = settings;

            this.ServiceListener = new ServiceListener(this);
            this.SessionPool = new SessionPool(this);
            this.SessionManager = new SessionManager(this);
            this.PacketManager = new PacketManager(this);
        }

        public bool Start()
        {
            var listenerResult = this.ServiceListener.Start();
            if (listenerResult)
                StaticLogger.Instance.Info($"{this.Settings.Name} is listening on {this.Settings.IP}:{this.Settings.Port}");

            var poolResult = this.SessionPool.Start();
            if (poolResult)
                StaticLogger.Instance.Info($"{this.Settings.Name} started with {this.SessionPool.ThreadCount} thread(s)");

            return listenerResult && poolResult;
        }

        public void Stop()
        {
            this.ServiceListener.Stop();
            this.SessionPool.Stop();
        }
    }
}
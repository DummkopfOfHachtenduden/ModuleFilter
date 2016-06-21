using Silkroad.Framework.Utility;
using System;
using System.Net;
using System.Net.Sockets;

namespace Silkroad.Framework.Common
{
    public class ServiceListener
    {
        private const int MAX_BACKLOG = 10;

        private Service _service;
        private Socket _listener;

        public ServiceListener(Service service)
        {
            _service = service;
        }

        public bool Start()
        {
            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                var localEP = new IPEndPoint(IPAddress.Parse(_service.Settings.IP), _service.Settings.Port);
                _listener.Bind(localEP);
                _listener.Listen(MAX_BACKLOG);

                _listener.BeginAccept(BeginAcceptCallback, null);
                return true;
            }
            catch (Exception ex)
            {
                StaticLogger.Instance.Fatal(ex, $"{nameof(ServiceListener)}->{Caller.GetMemberName()}:");
                return false;
            }
        }

        public void Stop()
        {
            if (_listener != null)
                _listener.Close();
        }

        private void BeginAcceptCallback(IAsyncResult ar)
        {
            try
            {
                var client = _listener.EndAccept(ar);
                var result = _service.SessionManager.Create(client);
                if (!result && client != null)
                    client.Close();

                _listener.BeginAccept(BeginAcceptCallback, _listener);
            }
            catch (Exception ex)
            {
                StaticLogger.Instance.Fatal(ex, $"{nameof(ServiceListener)}->{Caller.GetMemberName()}:");
            }
        }
    }
}
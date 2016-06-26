using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace Silkroad.Framework.Common
{
    public class SessionManager
    {
        private readonly object _syncLock;

        private Service _service;

        private int _sessionCounter;
        private List<Session> _sessions;

        public IReadOnlyList<Session> Sessions
        {
            get
            {
                return _sessions;
            }
        }

        public SessionManager(Service service)
        {
            _syncLock = new object();
            _service = service;

            _sessions = new List<Session>();
        }

        internal bool Create(Socket client)
        {
            lock (_syncLock)
            {
                var id = Interlocked.Increment(ref _sessionCounter);

                var session = new Session(_service, client, id);

                _sessions.Add(session);

                _service.SessionPool.RunInThread(session);
            }
            return true;
        }

        internal void Destroy(Session session)
        {
            lock (_syncLock)
            {
                session.Disconnect(true);
                _sessions.Remove(session);
            }
        }
    }
}
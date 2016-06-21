using Silkroad.Framework.Utility;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Silkroad.Framework.Common
{
    public sealed class SessionPool
    {
        private Service _service;

        private int _threadCount;

        public int ThreadCount
        {
            get
            {
                return _threadCount;
            }
        }

        private Thread[] _sessionThreads;

        private List<SessionPoolWorkItem> _workItems;

        private bool _running;

        private Random _rand;

        private readonly object _syncRoot;

        public SessionPool(Service service)
        {
            _syncRoot = new object();
            _service = service;
            _running = false;
            _rand = new Random();
        }

        public bool Start()
        {
            if (_running)
                return false;

            //1 session processing thread for each CPU core
            //_threadCount = Environment.ProcessorCount;
            _threadCount = 1;

            _sessionThreads = new Thread[_threadCount];

            //Contains numbers which describe current count of sessions which
            //were processed by specific thread. Thread with lowest should be chosen
            //for accepting new work.

            //Needs synchronization (!!!)
            _workItems = new List<SessionPoolWorkItem>();

            //Threads loop while _running is true, so, it has to be
            //assigned BEFORE we start them
            _running = true;

            //Initialize
            for (int i = 0; i < _threadCount; i++)
            {
                _sessionThreads[i] = new Thread(SessionWorker);

                //Allow aborting those threads on application exit with no problem
                _sessionThreads[i].IsBackground = true;

                //i = threadIndex
                _sessionThreads[i].Start(i);
            }

            //_threadTickPoller = new Timer(ThreadPollTimerTickHandler, null, 0, 1000);
            return true;
        }

        public void Stop()
        {
            if (!_running)
                return;

            _running = false;

            lock (_syncRoot)
            {
                _workItems.Clear();
            }

            for (int i = 0; i < _threadCount; i++)
            {
                try
                {
                    _sessionThreads[i].Abort();
                }
                catch (Exception ex)
                {
                    StaticLogger.Instance.Fatal(ex, $"{nameof(SessionPool)}->{Caller.GetMemberName()}:");
                }
            }
        }

        public void RunInThread(Session session)
        {
            SessionPoolWorkItem workItem = new SessionPoolWorkItem(session, _rand.Next(0, _threadCount));

            lock (_syncRoot)
            {
                _workItems.Add(workItem);
            }
        }

        //NOTE: Work items returned are deleted from _workItems collection
        //since they are considered as dispatched to thread
        private List<Session> GetWorkForThreadIndex(int threadIndex)
        {
            List<Session> result = new List<Session>();

            List<SessionPoolWorkItem> toDelete = new List<SessionPoolWorkItem>();

            lock (_syncRoot)
            {
                for (int i = 0; i < _workItems.Count; i++)
                {
                    if (_workItems[i].ThreadIndex == threadIndex)
                    {
                        result.Add(_workItems[i].Session);
                        toDelete.Add(_workItems[i]);
                    }
                }

                for (int i = 0; i < toDelete.Count; i++)
                {
                    _workItems.Remove(toDelete[i]);
                }
            }
            return result;
        }

        private void SessionWorker(object threadIndex)
        {
            int myThreadIndex = (int)(threadIndex);

            //List<Session> myWorkItems;
            while (_running)
            {
                var myWorkItems = GetWorkForThreadIndex(myThreadIndex);
                if (myWorkItems.Count != 0)
                {
                    for (int i = 0; i < myWorkItems.Count; i++)
                    {
                        bool result = myWorkItems[i].Run();
                    }
                }
                Thread.Sleep(1);
            }
        }
    }
}
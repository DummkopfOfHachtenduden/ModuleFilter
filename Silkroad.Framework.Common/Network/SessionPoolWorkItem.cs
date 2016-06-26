namespace Silkroad.Framework.Common
{
    internal struct SessionPoolWorkItem
    {
        //Session thats needs to run
        public Session Session;

        //Index of thread in which session will run
        public int ThreadIndex;

        public SessionPoolWorkItem(Session session, int threadIndex)
        {
            this.Session = session;
            this.ThreadIndex = threadIndex;
        }
    }
}
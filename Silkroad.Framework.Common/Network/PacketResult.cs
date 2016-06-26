using Silkroad.Framework.Common.Security;
using System.Collections;
using System.Collections.Generic;

namespace Silkroad.Framework.Common
{
    public struct PacketResult : IEnumerable<Packet>
    {
        #region Fields (Static)

        public static PacketResult None = new PacketResult(PacketResultAction.None);
        public static PacketResult Ignore = new PacketResult(PacketResultAction.Ignore);
        public static PacketResult Disconnect = new PacketResult(PacketResultAction.Disconnect);

        #endregion Fields (Static)

        #region Fields

        private PacketResultAction _action;
        private List<Packet> _packets;

        #endregion Fields

        #region Properties

        public PacketResultAction Action
        {
            get
            {
                return _action;
            }
        }

        #endregion Properties

        #region Constructor

        public PacketResult(PacketResultAction action)
        {
            _action = action;
            _packets = new List<Packet>();
        }

        public PacketResult(PacketResultAction action, IEnumerable<Packet> packets)
        {
            _action = action;
            _packets = new List<Packet>(packets);
        }

        #endregion Constructor

        #region IEnumerable

        public Packet this[int index]
        {
            get
            {
                return _packets[index];
            }
        }

        public IEnumerator<Packet> GetEnumerator()
        {
            return _packets.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion IEnumerable

        #region Methods

        public void Add(Packet packet)
        {
            _packets.Add(packet);
        }

        public void Add(IEnumerable<Packet> packets)
        {
            _packets.AddRange(packets);
        }

        #endregion Methods
    }
}
using Silkroad.Framework.Common.Security;
using Silkroad.Framework.Utility;
using System;
using System.Collections.Generic;

namespace Silkroad.Framework.Common
{
    public class PacketManager
    {
        private Service _container;

        private Dictionary<ushort, Func<Session, Packet, PacketResult>> _moduleHandler;
        private Dictionary<ushort, Func<Session, Packet, PacketResult>> _certificatorHandler;

        public PacketManager(Service container)
        {
            _container = container;
            _moduleHandler = new Dictionary<ushort, Func<Session, Packet, PacketResult>>();
            _certificatorHandler = new Dictionary<ushort, Func<Session, Packet, PacketResult>>();
        }

        public void AddModuleHandler(ushort opcode, Func<Session, Packet, PacketResult> func)
        {
            _moduleHandler.Add(opcode, func);
        }

        public void AddCertificatorHandler(ushort opcode, Func<Session, Packet, PacketResult> func)
        {
            _certificatorHandler.Add(opcode, func);
        }

        internal PacketResult Handle(PacketSource source, Session session, Packet packet)
        {
            switch (source)
            {
                case PacketSource.Certificator:
                    if (_certificatorHandler.ContainsKey(packet.Opcode))
                    {
                        var handler = _certificatorHandler[packet.Opcode];
                        return handler.Invoke(session, packet);
                    }
                    break;

                case PacketSource.Module:
                    if (_moduleHandler.ContainsKey(packet.Opcode))
                    {
                        var handler = _moduleHandler[packet.Opcode];
                        return handler.Invoke(session, packet);
                    }
                    break;
            }

            if (StaticLogger.Instance.IsTraceEnabled)
            {
                var packet_bytes = packet.GetBytes();
                StaticLogger.Instance.Warn("[{7}][{0:X4}][{1} bytes]{2}{3}{4}{5}{6}", packet.Opcode, packet_bytes.Length, packet.Encrypted ? "[Encrypted]" : "", packet.Massive ? "[Massive]" : "", Environment.NewLine, packet_bytes.HexDump(), Environment.NewLine, source);
            }
            return PacketResult.None;
        }
    }
}
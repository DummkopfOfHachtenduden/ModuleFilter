using Silkroad.Framework.Common.Security;
using Silkroad.Framework.Utility;
using System;
using System.Net.Sockets;

namespace Silkroad.Framework.Common
{
    public class Session
    {
        //NOTE:
        //CERTIFICATOR = SERVER
        //MODULE = CLIENT

        private const int MAX_BUFFER = 8192;

        #region Fields

        private readonly object _syncLock;
        private bool _destroyed;

        private Service _service;

        private Socket _clientSocket;
        private Socket _certificatorSocket;

        private byte[] _clientBuffer;
        private byte[] _certificatorBuffer;

        private SecurityManager _clientSecurity;
        private SecurityManager _certificatorSecurity;

        private SessionState _state;

        #endregion Fields

        #region Properties

        public SessionState State
        {
            get { return _state; }
            set { _state = value; }
        }

        #endregion Properties

        public Session(Service service, Socket moduleSocket, int sessionID)
        {
            _syncLock = new object();

            //pass container
            _service = service;

            //pass socket
            _clientSocket = moduleSocket;

            //create buffers
            _clientBuffer = new byte[MAX_BUFFER];
            _certificatorBuffer = new byte[MAX_BUFFER];

            //create security
            _clientSecurity = new SecurityManager();

            //generate module security
            _clientSecurity.GenerateSecurity(_service.Settings.Blowfish,
                                             _service.Settings.SecurityBytes,
                                             _service.Settings.Handshake);

            _certificatorSecurity = new SecurityManager();
            _certificatorSecurity.ChangeIdentity(_service.Settings.Type.ToString(), 0);

            //create state
            _state = new SessionState();
            _state.ID = sessionID;
        }

        public bool Run()
        {
            _certificatorSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _certificatorSocket.Connect(_service.Settings.CertificatorIP, _service.Settings.CertificatorPort);

            if (_certificatorSocket.Connected)
            {
                this.BeginReceiveFromCertificator();
                this.BeginReceiveFromClient();

                return true;
            }
            else
            {
                this.Disconnect();
                return false;
            }
        }

        internal void Disconnect(bool suppressDestroy = false)
        {
            lock (_syncLock)
            {
                if (!_destroyed)
                {
                    _destroyed = true;

                    if (_clientSocket != null)
                        _clientSocket.Close();

                    if (_certificatorSocket != null)
                        _certificatorSocket.Close();
                }

                if (!suppressDestroy)
                {
                    _service.SessionManager.Destroy(this);
                }
            }
        }

        #region Certificator

        private void BeginReceiveFromCertificator()
        {
            try
            {
                _certificatorSocket.BeginReceive(_certificatorBuffer, 0, _certificatorBuffer.Length, SocketFlags.None, BeginReceiveFromCertificatorCallback, null);
            }
            catch (Exception ex)
            {
                StaticLogger.Instance.Error(ex, $"{nameof(Session)}->{Caller.GetMemberName()}:");
                this.Disconnect();
            }
        }

        private void BeginReceiveFromCertificatorCallback(IAsyncResult ar)
        {
            try
            {
                var nReceived = _certificatorSocket.EndReceive(ar);
                if (nReceived == 0)
                {
                    StaticLogger.Instance.Fatal($"{Caller.GetMemberName()}: 0 bytes received!");
                    this.Disconnect();
                    return;
                }

                _certificatorSecurity.Recv(_certificatorBuffer, 0, nReceived);
                var packets = _certificatorSecurity.TransferIncoming();
                if (packets != null)
                {
                    for (int i = 0; i < packets.Count; i++)
                    {
                        var packet = packets[i];
#if TRACE
                        if (StaticLogger.Instance.IsTraceEnabled)
                            StaticLogger.Instance.Trace("[S->P][{0:X4}][{1} bytes]{2}{3}{4}{5}{6}", packet.Opcode, packet.Length, packet.Encrypted ? "[Encrypted]" : "", packet.Massive ? "[Massive]" : "", Environment.NewLine, packet.GetBytes().HexDump(), Environment.NewLine);
#endif
                        if (packet.Opcode == 0x5000 || packet.Opcode == 0x9000)
                            continue;

                        var result = _service.PacketManager.Handle(PacketSource.Certificator, this, packet);
                        switch (result.Action)
                        {
                            case PacketResultAction.Ignore:
                                return;

                            case PacketResultAction.Disconnect:
                                this.Disconnect();
                                return;

                            case PacketResultAction.Replace:
                                foreach (var replacedPacket in result)
                                    _clientSecurity.Send(replacedPacket);
                                continue;

                            case PacketResultAction.Response:
                                foreach (var replacedPacket in result)
                                    _certificatorSecurity.Send(replacedPacket);
                                continue;
                        }

                        _clientSecurity.Send(packet);
                    }
                }

                //this.TransferToClient();
                this.TransferTo(_clientSecurity, _clientSocket);
                this.BeginReceiveFromCertificator();
            }
            catch (Exception ex)
            {
                StaticLogger.Instance.Error(ex, $"{nameof(Session)}->{Caller.GetMemberName()}:");
                this.Disconnect();
            }
        }

        private void TransferToCertificator()
        {
            try
            {
                var kvp = _certificatorSecurity.TransferOutgoing();
                if (kvp != null)
                {
                    for (int i = 0; i < kvp.Count; i++)
                    {
                        if (_destroyed)
                            return;

#if TRACE
                        var packet = kvp[i].Value;
                        if (StaticLogger.Instance.IsTraceEnabled)
                            StaticLogger.Instance.Trace("[P->S][{0:X4}][{1} bytes]{2}{3}{4}{5}{6}", packet.Opcode, packet.Length, packet.Encrypted ? "[Encrypted]" : "", packet.Massive ? "[Massive]" : "", Environment.NewLine, packet.GetBytes().HexDump(), Environment.NewLine);
#endif

                        _certificatorSocket.BeginSend(kvp[i].Key.Buffer, 0, kvp[i].Key.Buffer.Length, SocketFlags.None, BeginSendToCertificatorCallback, null);
                    }
                }
            }
            catch (Exception ex)
            {
                StaticLogger.Instance.Error(ex, $"{nameof(Session)}->{Caller.GetMemberName()}:");
                this.Disconnect();
            }
        }

        private void BeginSendToCertificatorCallback(IAsyncResult ar)
        {
            try
            {
                _certificatorSocket.EndSend(ar);
            }
            catch (Exception ex)
            {
                StaticLogger.Instance.Error(ex, $"{nameof(Session)}->{Caller.GetMemberName()}:");
                this.Disconnect();
            }
        }

        public void SendToCertificator(Packet packet)
        {
            if (_destroyed)
                return;

            try
            {
                _certificatorSecurity.Send(packet);
                TransferToCertificator();
            }
            catch (Exception ex)
            {
                StaticLogger.Instance.Error(ex, $"{nameof(Session)}->{Caller.GetMemberName()}:");
                this.Disconnect();
            }
        }

        #endregion Certificator

        #region Module

        private void BeginReceiveFromClient()
        {
            try
            {
                _clientSocket.BeginReceive(_clientBuffer, 0, _clientBuffer.Length, SocketFlags.None, BeginReceiveFromClientCallback, null);
            }
            catch (Exception ex)
            {
                StaticLogger.Instance.Error(ex, $"{nameof(Session)}->{Caller.GetMemberName()}:");
                this.Disconnect();
            }
        }

        private void BeginReceiveFromClientCallback(IAsyncResult ar)
        {
            try
            {
                var nReceived = _clientSocket.EndReceive(ar);
                if (nReceived == 0)
                {
                    StaticLogger.Instance.Fatal($"{Caller.GetMemberName()}: 0 bytes received!");
                    this.Disconnect();
                    return;
                }

                _clientSecurity.Recv(_clientBuffer, 0, nReceived);
                var packets = _clientSecurity.TransferIncoming();
                if (packets != null)
                {
                    for (int i = 0; i < packets.Count; i++)
                    {
                        var packet = packets[i];
#if TRACE
                        if (StaticLogger.Instance.IsTraceEnabled)
                            StaticLogger.Instance.Trace("[C->P][{0:X4}][{1} bytes]{2}{3}{4}{5}{6}", packet.Opcode, packet.Length, packet.Encrypted ? "[Encrypted]" : "", packet.Massive ? "[Massive]" : "", Environment.NewLine, packet.GetBytes().HexDump(), Environment.NewLine);
#endif
                        if (packet.Opcode == 0x5000 || packet.Opcode == 0x9000 || packet.Opcode == 0x2001)
                            continue;

                        var result = _service.PacketManager.Handle(PacketSource.Module, this, packet);
                        switch (result.Action)
                        {
                            case PacketResultAction.Ignore:
                                continue;

                            case PacketResultAction.Disconnect:
                                this.Disconnect();
                                return;

                            case PacketResultAction.Replace:
                                foreach (var replacedPacket in result)
                                    _certificatorSecurity.Send(replacedPacket);
                                continue;

                            case PacketResultAction.Response:
                                foreach (var replacedPacket in result)
                                    _clientSecurity.Send(replacedPacket);
                                continue;
                        }

                        _certificatorSecurity.Send(packet);
                    }
                }

                //this.TransferToCertificator();
                this.TransferTo(_certificatorSecurity, _certificatorSocket);
                this.BeginReceiveFromClient();
            }
            catch (Exception ex)
            {
                StaticLogger.Instance.Error(ex, $"{nameof(Session)}->{Caller.GetMemberName()}:");
                this.Disconnect();
            }
        }

        private void TransferToClient()
        {
            try
            {
                var kvp = _clientSecurity.TransferOutgoing();
                if (kvp != null)
                {
                    for (int i = 0; i < kvp.Count; i++)
                    {
                        if (_destroyed)
                            return;

#if TRACE
                        var packet = kvp[i].Value;
                        if (StaticLogger.Instance.IsTraceEnabled)
                            StaticLogger.Instance.Trace("[P->C][{0:X4}][{1} bytes]{2}{3}{4}{5}{6}", packet.Opcode, packet.Length, packet.Encrypted ? "[Encrypted]" : "", packet.Massive ? "[Massive]" : "", Environment.NewLine, packet.GetBytes().HexDump(), Environment.NewLine);
#endif

                        _clientSocket.BeginSend(kvp[i].Key.Buffer, 0, kvp[i].Key.Buffer.Length, SocketFlags.None, BeginSendToClientCallback, null);
                    }
                }
            }
            catch (Exception ex)
            {
                StaticLogger.Instance.Error(ex, $"{nameof(Session)}->{Caller.GetMemberName()}:");
                this.Disconnect();
            }
        }

        private void BeginSendToClientCallback(IAsyncResult ar)
        {
            try
            {
                _clientSocket.EndSend(ar);
            }
            catch (Exception ex)
            {
                StaticLogger.Instance.Error(ex, $"{nameof(Session)}->{Caller.GetMemberName()}:");
                this.Disconnect();
            }
        }

        public void SendToClient(Packet packet)
        {
            if (_destroyed)
                return;

            try
            {
                _clientSecurity.Send(packet);
                TransferToClient();
            }
            catch (Exception ex)
            {
                StaticLogger.Instance.Error(ex, $"{nameof(Session)}->{Caller.GetMemberName()}:");
                this.Disconnect();
            }
        }

        #endregion Module

        #region Generic

        private void TransferTo(SecurityManager manager, Socket socket)
        {
            try
            {
                var kvp = manager.TransferOutgoing();
                if (kvp != null)
                {
                    for (int i = 0; i < kvp.Count; i++)
                    {
                        if (_destroyed)
                            return;

#if TRACE
                        var packet = kvp[i].Value;
                        if (StaticLogger.Instance.IsTraceEnabled)
                            StaticLogger.Instance.Trace("[P->{7}][{0:X4}][{1} bytes]{2}{3}{4}{5}{6}", packet.Opcode, packet.Length, packet.Encrypted ? "[Encrypted]" : "", packet.Massive ? "[Massive]" : "", Environment.NewLine, packet.GetBytes().HexDump(), Environment.NewLine, manager.IdentityName);
#endif

                        socket.BeginSend(kvp[i].Key.Buffer, 0, kvp[i].Key.Buffer.Length, SocketFlags.None, BeginSendCallback, socket);
                    }
                }
            }
            catch (Exception ex)
            {
                StaticLogger.Instance.Error(ex, $"{nameof(Session)}->{Caller.GetMemberName()}:");
                this.Disconnect();
            }
        }

        private void BeginSendCallback(IAsyncResult ar)
        {
            try
            {
                var socket = ar.AsyncState as Socket;
                socket.EndSend(ar);
            }
            catch (Exception ex)
            {
                StaticLogger.Instance.Error(ex, $"{nameof(Session)}->{Caller.GetMemberName()}:");
                this.Disconnect();
            }
        }

        #endregion Generic
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartDeviceLink.Net.Converters;
using SmartDeviceLink.Net.Exceptions;
using SmartDeviceLink.Net.Logging;
using SmartDeviceLink.Net.Protocol;
using SmartDeviceLink.Net.Protocol.Enums;
using SmartDeviceLink.Net.Protocol.Session;
using SmartDeviceLink.Net.Rpc.Base;
using SmartDeviceLink.Net.Transport;
using SmartDeviceLink.Net.Transport.Enums;
using SmartDeviceLink.Net.Transport.Interfaces;

namespace SmartDeviceLink.Net.SdlService
{
    /// <summary>
    ///     Based on sdl_Android Sdl Proxy Base
    /// </summary>
    public class SdlClient : IDisposable
    {
        private ILogger _logger => Logger.SdlLogger;
        private readonly ITransport _transport;
        private readonly WiProProtocolManager _protocol;
        private List<Session> _sessions = null;
        private bool isDisposed;

        public SdlClient(ITransport transport)
        {
            _sessions = new List<Session>();
            _transport = transport;
            _transport.OnRecievedPacket = PacketRecieved;
            _protocol = new WiProProtocolManager();
        }

        /// <summary>
        /// Move This to Protocol Manager
        /// </summary>
        public async Task StartSessionAsync(SessionType type)
        {
            var _session = GetSession(type);
            await _transport.SendAsync(new TransportPacket()
            {
                Version = 4,
                FrameType = FrameType.Control,
                ControlFrameInfo = FrameInfo.StartSession,
                SessionId = 0,
                MessageId = 1,
                ServiceType = type,
                IsEncrypted = false
            });
        }
        /// <summary>
        /// Move This to Protocol Manager
        /// </summary>
        private Session GetSession(SessionType type)
        {
            if (_sessions.All(x => x.SessionType != type))
            {
                var session = new Session();
                session.SessionType = type;
                _sessions.Add(session);
                return session;
            }

            return _sessions.First(x => x.SessionType == type);
        }
        /// <summary>
        /// Refactor to _protocol.SendAsync(request.ToProtocolMessage())
        /// </summary>
        public async Task SendAsync(RpcRequest request)
        {
            var protocolMessage = request.ToProtocolMessage();
            var session = GetSession(SessionType.Rpc);
            protocolMessage.SessionId = session.SessionId;
            _logger.LogVerbose("Created Protocol Message",protocolMessage);
            _logger.LogVerbose("Bytes: " + protocolMessage.JsonSize);
            protocolMessage.Version = 4;// hard coded... pull from session?
            var transportPackets = _protocol.CreateTransportPackets(protocolMessage);
            
            foreach (var pack in transportPackets)
            {
                
                await _transport.SendAsync(pack);
            }
        }
        /// <summary>
        /// Move to protocol
        /// </summary>
        /// <param name="packet"></param>
        private void PacketRecieved(TransportPacket packet)
        {
            _logger.LogVerbose("Packet Recieved", packet);
            
            if (packet.FrameType == FrameType.Control)
            {
                _logger.LogVerbose("Packet is a control frame");
                //handle start session, grab session id
                var session = GetSession(packet.ServiceType);
                session.SessionId = packet.SessionId;
            }
            else
            {
                var pm = packet.ToProtocolMessage();
                _logger.LogVerbose("To Protocol message", packet.ToProtocolMessage());
                if (pm.JsonSize > 0)
                    _logger.LogVerbose(Encoding.ASCII.GetString(pm.Payload));
            }
        }

        public void Dispose()
        {
            if (!isDisposed)
                _transport.Dispose();
            isDisposed = true;
        }
    }
}
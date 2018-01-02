using System;
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
        private readonly WiProProtocol _protocol;
        private Session _session = null;
        private bool isDisposed;

        public SdlClient(ITransport transport)
        {
            _transport = transport;
            _transport.OnRecievedPacket = PacketRecieved;
            _protocol = new WiProProtocol();
        }

        public async Task StartSessionAsync(SessionType type)
        {
            _session = new Session();
            _session.SessionType = type;
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

        public async Task SendAsync(RpcRequest request)
        {
            if(_session == null)
            { throw new SessionException("Session does not exist, StartSession() must be called prior to sending RpcRequests");}
            var protocolMessage = request.ToProtocolMessage();
            _logger.LogVerbose("Created Protocol Message",protocolMessage);
            _logger.LogVerbose("Bytes: " + protocolMessage.JsonSize);
            var transportPackets = _protocol.CreateTransportPackets(protocolMessage);
            foreach (var pack in transportPackets)
            {
                await _transport.SendAsync(pack);
            }
        }

        private void PacketRecieved(TransportPacket packet)
        {
            _logger.LogVerbose("Packet Recieved", packet);
            
            if (packet.FrameType == FrameType.Control)
            {
                _logger.LogVerbose("Packet is a control frame");
                //handle start session, grab session id
            }
            else
            {
                var pm = packet.ToProtocolMessage();
                _logger.LogVerbose("To Protocol message", packet.ToProtocolMessage());
                if (pm.JsonSize > 0)
                    _logger.LogVerbose(Encoding.Unicode.GetString(pm.Payload));
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
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SmartDeviceLink.Net.Converters;
using SmartDeviceLink.Net.Logging;
using SmartDeviceLink.Net.Protocol.Enums;
using SmartDeviceLink.Net.Transport;
using SmartDeviceLink.Net.Transport.Enums;
using SmartDeviceLink.Net.Transport.Interfaces;

namespace SmartDeviceLink.Net.Protocol
{
    public class WiProProtocolManager :IDisposable
    {
        private readonly ITransport _transport;

        private ILogger _logger => Logger.SdlLogger;
        public static readonly int V1_V2_MTU_SIZE = 1500;
        public static readonly int V3_V4_MTU_SIZE = 131072;
        public static readonly int V1_HEADER_SIZE = 8;
        public static readonly int V2_HEADER_SIZE = 12;

        private int _messageId = 1;
        private List<Session.Session> _sessions = null;
        private TaskCompletionSource<ProtocolMessage> _afterSendRecieveCompletion;
        private bool isDisposed;

        //TODO: Find where this is being set
        private int protocolVersion = 1;// this will eventually be set upon  start session

        public WiProProtocolManager( ITransport transport)
        {
            _transport = transport;
            _transport.OnRecievedPacket = PacketRecieved;
            _sessions = new List<Session.Session>();
        }

        
        /// <summary>
        /// Refactor to _protocol.SendAsync(request.ToProtocolMessage())
        /// </summary>
        public async Task<ProtocolMessage> SendAsync(ProtocolMessage protocolMessage)
        {
            var session = GetSession(protocolMessage.ServiceType);
            if (session.SessionId == 0) await StartSessionAsync(protocolMessage.ServiceType);
            return await SendAfterSessionAsync(session,protocolMessage);
        }

        private object lockobj = new object();
        private async Task<ProtocolMessage> SendAfterSessionAsync(Session.Session session1,ProtocolMessage protocolMessage)
        {
            try
            {
                if (_afterSendRecieveCompletion != null)
                    await _afterSendRecieveCompletion.Task;
                lock(lockobj)
                    _afterSendRecieveCompletion = new TaskCompletionSource<ProtocolMessage>();


                protocolMessage.SessionId = session1.SessionId;
                _logger.LogVerbose("Created Protocol Message", protocolMessage);
                _logger.LogVerbose("Bytes: " + protocolMessage.JsonSize);
                protocolMessage.Version = 4; // hard coded... pull from session?
                var transportPackets = CreateTransportPackets(protocolMessage);

                foreach (var item in transportPackets)
                { 
                    await _transport.SendAsync(item);
                }
                
                session1.LastTxRx = DateTime.Now;
                var response = await _afterSendRecieveCompletion.Task;
                session1.LastTxRx = DateTime.Now;
                if (response == null) return null;
                session1.SessionId = response.SessionId;
                if (response.Version > 1) session1.Version = response.Version;
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError("broke", e: e);
            }

            return null;
        }

        /// <summary>
        /// Packet handler
        /// </summary>
        /// <param name="packet">Message as it comes back from the packet parser class</param>
        private void PacketRecieved(TransportPacket packet)
        {
            _logger.LogVerbose("Packet Recieved", packet);

            if (packet.FrameType == FrameType.Control)
            {
                _logger.LogVerbose("Packet is a control frame");
                //handle start session, grab session id
                var session = GetSession(packet.ServiceType);
                session.SessionId = packet.SessionId;
                if (packet.ControlFrameInfo == FrameInfo.EndService)
                {
                    _logger.LogError("Connection Closed",packet);
                    _afterSendRecieveCompletion.TrySetException(
                        new Exception("Connection Closed by remote"));
                }
            }
            else
            {
                var pm = packet.ToProtocolMessage();
                _logger.LogVerbose("To Protocol message", packet.ToProtocolMessage());
                if (pm.JsonSize > 0)
                    _logger.LogVerbose(Encoding.ASCII.GetString(pm.Payload));
                _afterSendRecieveCompletion.SetResult(pm);
            }
        }

        /// <summary>
        /// Move This to Protocol Manager
        /// </summary>
        public async Task StartSessionAsync(ServiceType type)
        {
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
        private Session.Session GetSession(ServiceType type)
        {
            if (_sessions.All(x => x.ServiceType != type))
            {
                var session = new Session.Session();
                session.ServiceType = type;
                _sessions.Add(session);
                return session;
            }

            return _sessions.First(x => x.ServiceType == type);
        }

        /// <summary>
        /// Convert a single protocol message into a corresponding number of transport packets.
        /// transport.SendAsync([TransportPacketData[ProtocolPacketdata]])
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public IEnumerable<TransportPacket> CreateTransportPackets(ProtocolMessage message)
        {
            var tPackets = new List<TransportPacket>();
            message.SessionId = 1; // This is a stub, this should be found from the eventual implementation of the session object
            var messageBytes = message.ToProtocolFrame();
            // for now secured data is not supported

            // data is not going to be processed in one of two ways
            // first is single send
            var p = SinglePacket(message, messageBytes, _messageId++);
            _logger.LogVerbose("CreatedSinglePacket",p);
            tPackets.Add(p);

            return tPackets;
        }

        private TransportPacket SinglePacket(ProtocolMessage protocolPacket,byte[] protocolPacketBytes,int messageId)
        {
            var transportPacket = new TransportPacket();
            transportPacket.MessageId = messageId;
            transportPacket.Version = protocolPacket.Version;
            transportPacket.IsEncrypted = false;
            transportPacket.FrameType = FrameType.Single;
            transportPacket.ServiceType =  protocolPacket.ServiceType;
            transportPacket.SessionId = protocolPacket.SessionId;
            transportPacket.DataSize = protocolPacketBytes?.Length ?? 0;
            transportPacket.Payload = protocolPacketBytes;
            transportPacket.ControlFrameInfo = FrameInfo.Heartbeat_FinalConsecutiveFrame_Reserved;
            transportPacket.PriorityCoefficient = protocolPacket.PriorityCoefficient;
            return transportPacket;
        }

        public void Dispose()
        {
            if (!isDisposed)
                _transport.Dispose();
            isDisposed = true;
        }
    }
}

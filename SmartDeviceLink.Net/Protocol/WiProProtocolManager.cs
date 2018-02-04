using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SmartDeviceLink.Net.Common;
using SmartDeviceLink.Net.Converters;
using SmartDeviceLink.Net.Exceptions;
using SmartDeviceLink.Net.Logging;
using SmartDeviceLink.Net.Protocol.Enums;
using SmartDeviceLink.Net.Protocol.Models;
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
            if (session.SessionId == 0) throw new SessionUnregisteredException($"Session for {protocolMessage.ServiceType} has not been registered yet");
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
                protocolMessage.Version = session1.Version;
                var transportPackets = CreateTransportPackets(protocolMessage);

                foreach (var item in transportPackets)
                { 
                    await _transport.SendAsync(item);
                }
                
                session1.LastTxRx = DateTime.Now;
                var response = await _afterSendRecieveCompletion.Task;
                session1.LastTxRx = DateTime.Now;
                if (response == null) return null;
                if(response.SessionId > 0)
                    session1.SessionId = response.SessionId;
                if (response.Version > 1) session1.Version = response.Version;
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError("Unable to send message to HMI", e: e);
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
                HandleControlFrame(packet);
            else
                HandleRpc(packet);
            
        }

        private void HandleRpc(TransportPacket packet)
        {
            var pm = packet.ToProtocolMessage();
            _logger.LogVerbose("To Protocol message", packet.ToProtocolMessage());
            if (pm.JsonSize > 0)
                _logger.LogVerbose(Encoding.ASCII.GetString(pm.Payload));
            _afterSendRecieveCompletion.SetResult(pm);
        }

        private void HandleControlFrame(TransportPacket packet)
        {
            _logger.LogVerbose("Packet is a control frame");
            //handle start session, grab session id
            var session = GetSession(packet.ServiceType);
            session.SessionId = packet.SessionId;
            if(packet.Version > 1) session.Version = packet.Version;
            if (packet.ControlFrameInfo == FrameInfo.EndService)
            {
                _logger.LogError("Connection Closed", packet);
                _afterSendRecieveCompletion.TrySetException(
                    new Exception("Connection Closed by remote"));
            }
        }


        public async Task StartSessionAsync(ServiceType type)
        {
            var bson = Bson.BsonConvert.ToBson(new Dictionary<string, string> {{"protocolVersion", "5.0.0"}});
            await _transport.SendAsync(new TransportPacket()
            {
                Version = 5,
                FrameType = FrameType.Control,
                ControlFrameInfo = FrameInfo.StartSession,
                SessionId = 0,
                MessageId = 1,
                ServiceType = type,
                IsEncrypted = false,
                Payload = bson,
                DataSize = bson.Length
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
            List<TransportPacket> tPackets = null;
            var session = GetSession(message.ServiceType);
            message.SessionId = session.SessionId; 
            var messageBytes = message.ToProtocolFrame();

            var mtuSize = session.Version < 3? V1_V2_MTU_SIZE : V3_V4_MTU_SIZE;
            
            if (message.JsonSize > mtuSize)
            {
                tPackets = CreateMultiPacket(message, messageBytes, mtuSize);
            }
            else
            {
                var p = SinglePacket(message, messageBytes, _messageId++);
                _logger.LogVerbose("CreatedSinglePacket", p);
                tPackets = new List<TransportPacket>();
                tPackets.Add(p);
            }
            return tPackets;
        }

        private List<TransportPacket> CreateMultiPacket(ProtocolMessage message, byte[] messageBytes, int mtuSize)
        {
            List<TransportPacket> tPackets = new List<TransportPacket>();
            var messageid = _messageId++;
            var firstFramePacket = new TransportPacket()
            {
                Version = message.Version,
                ServiceType = message.ServiceType,
                FrameType = FrameType.First,
                MessageId = messageid,
                Payload = BitConverter.GetBytes(message.JsonSize),
                DataSize = 8,
                PriorityCoefficient = message.PriorityCoefficient + 1,
                SessionId = message.SessionId,
                IsEncrypted = message.IsPayloadProtected
            };
            tPackets.Add(firstFramePacket);
            var splitPayload = messageBytes.Split(mtuSize).ToList();
            var lastPayload = splitPayload.Last();
            var frameSequenceNumber = 0;
            int priCoef = 1;
            foreach (var item in splitPayload)
            {
                frameSequenceNumber++;
                if (!item.Equals(lastPayload) && frameSequenceNumber == 0)
                {
                    frameSequenceNumber = 1; // for roll over 0 is reserved for last frame
                }
                else if (item.Equals(lastPayload))
                    frameSequenceNumber = 0;

                var consecPacket = new TransportPacket()
                {
                    Version = message.Version,
                    ServiceType = message.ServiceType,
                    FrameType = FrameType.First,
                    MessageId = messageid,
                    Payload = (byte[]) item,
                    DataSize = ((byte[]) item).Length,
                    PriorityCoefficient = message.PriorityCoefficient + 1 + priCoef++,
                    SessionId = message.SessionId,
                    IsEncrypted = message.IsPayloadProtected
                };
                tPackets.Add(consecPacket);
            }

            return tPackets;
        }

        private TransportPacket SinglePacket(ProtocolMessage protocolPacket,byte[] protocolPacketBytes,int messageId)
        {
            var transportPacket = new TransportPacket();
            transportPacket.MessageId = messageId;
            transportPacket.Version = protocolPacket.Version;
            transportPacket.IsEncrypted = protocolPacket.IsPayloadProtected;
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

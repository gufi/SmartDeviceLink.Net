using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SmartDeviceLink.Net.Common;
using SmartDeviceLink.Net.Converters;
using SmartDeviceLink.Net.Exceptions;
using SmartDeviceLink.Net.Logging;
using SmartDeviceLink.Net.Protocol.Enums;
using SmartDeviceLink.Net.Protocol.Models;
using SmartDeviceLink.Net.Rpc.Base;
using SmartDeviceLink.Net.Transport;
using SmartDeviceLink.Net.Transport.Enums;
using SmartDeviceLink.Net.Transport.Interfaces;

namespace SmartDeviceLink.Net.Protocol
{
    public class WiProProtocolManager :IDisposable
    {
        private readonly ITransport _transport;
        private readonly Action<ProtocolMessage> _eventHandler;

        private ILogger _logger => Logger.SdlLogger;
        public static readonly int V1_V2_MTU_SIZE = 1500;
        public static readonly int V3_V4_MTU_SIZE = 131072;
        public static readonly int V1_HEADER_SIZE = 8;
        public static readonly int V2_HEADER_SIZE = 12;

        private int _messageId = 1;
        private List<Session.Session> _sessions = null;
        private readonly CancellationTokenSource _heartbeatCancelToken;
        private readonly Task _heartBeatMonitor;
        private bool isDisposed;
        

        public WiProProtocolManager( ITransport transport, Action<ProtocolMessage> eventHandler)
        {
            _transport = transport;
            _eventHandler = eventHandler;
            _transport.OnRecievedPacket = PacketRecieved;
            _sessions = new List<Session.Session>();
            _heartbeatCancelToken = new CancellationTokenSource();
            
            _heartBeatMonitor = Task.Run(SendHeartBeat, _heartbeatCancelToken.Token);
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

        private Dictionary<ProtocolMessage, TaskCompletionSource<ProtocolMessage>> _protocolMessageManager = new Dictionary<ProtocolMessage, TaskCompletionSource<ProtocolMessage>>();
        private async Task<ProtocolMessage> SendAfterSessionAsync(Session.Session session,ProtocolMessage protocolMessage)
        {
            try
            {
                protocolMessage.SessionId = session.SessionId;
                protocolMessage.Version = session.Version;
                var afterSendRecieveCompletion = new TaskCompletionSource<ProtocolMessage>();
                _protocolMessageManager.Add(protocolMessage, afterSendRecieveCompletion);
                var transportPackets = CreateTransportPackets(protocolMessage);

                foreach (var item in transportPackets)
                { 
                    await _transport.SendAsync(item);
                }
                
                session.LastTxRx = DateTime.Now;
                var response = await afterSendRecieveCompletion.Task;
                session.LastTxRx = DateTime.Now;
                if (response == null) return null;
                if(response.SessionId > 0)
                    session.SessionId = response.SessionId;
                if (response.Version > 1) session.Version = response.Version;
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
            _logger.LogDebug("Packet Recieved", packet);
            var session = GetSession(packet.ServiceType);
            if (packet.Version > 1) session.Version = packet.Version;
            session.SessionId = packet.SessionId;
            if (packet.FrameType == FrameType.Control)
                HandleControlFrame(packet);
            else
                HandleRpc(packet);
            
        }

        private void HandleRpc(TransportPacket packet)
        {// This does not currently support consecutive packets
            var pm = packet.ToProtocolMessage();
            _logger.LogVerbose("To Protocol message", packet.ToProtocolMessage());
            if (pm.JsonSize > 0)
                _logger.LogVerbose(Encoding.ASCII.GetString(pm.Payload));
            var response = _protocolMessageManager.FirstOrDefault(x => x.Key.FunctionId == pm.FunctionId);

            if (response.Key != null)
            { 
                response.Value.SetResult(pm);
                _protocolMessageManager.Remove(response.Key);
            }
            else if (pm.FunctionId.ToString().StartsWith("On") || pm.FunctionId.ToString().Contains(".On"))
            {
                _logger.LogDebug("Event Based Packet",pm);
                _eventHandler(pm);
            }
        }

        private void HandleControlFrame(TransportPacket packet)
        {
            _logger.LogDebug("Packet is a control frame",packet);
            //handle start session, grab session id
            
            if (packet.ControlFrameInfo == FrameInfo.EndService)
            {
                _logger.LogError("Connection Closed", packet);
                foreach(var item in _protocolMessageManager)
                item.Value.TrySetException(
                    new Exception("Connection Closed by remote"));
            }
            else if (packet.ControlFrameInfo == FrameInfo.HeartBeatAck)
            {
                _logger.LogDebug("HeartBeat",packet);
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
                MessageId = _messageId++,
                ServiceType = type,
                IsEncrypted = false,
                Payload = bson,
                DataSize = bson.Length
            });
        }

        private async Task SendHeartBeat()
        {
            while (!_heartbeatCancelToken.IsCancellationRequested)
            {
                await Task.Delay(1000);
                foreach (var session in _sessions.Where(x =>x.SessionId > 0 && (DateTime.Now- x.LastTxRx).Seconds >= 8))
                {
                    await SendHeartBeat(session);
                }
            }
        }
        private async Task SendHeartBeat(Session.Session session)
        {
            await _transport.SendAsync(new TransportPacket()
            {
                Version = 5,
                FrameType = FrameType.Control,
                ControlFrameInfo = FrameInfo.Heartbeat_FinalConsecutiveFrame_Reserved,
                SessionId = session.SessionId,
                MessageId = _messageId++,
                ServiceType = session.ServiceType,
                IsEncrypted = false
            });
            session.LastTxRx = DateTime.Now;
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
            var messageId = ++_messageId;
            message.MessageId = messageId;
            if (message.JsonSize > mtuSize)
            {
                tPackets = CreateMultiPacket(message, messageBytes, mtuSize,messageId);
            }
            else
            {
                var p = SinglePacket(message, messageBytes, messageId);
                _logger.LogVerbose("CreatedSinglePacket", p);
                tPackets = new List<TransportPacket>();
                tPackets.Add(p);
            }
            return tPackets;
        }

        private List<TransportPacket> CreateMultiPacket(ProtocolMessage message, byte[] messageBytes, int mtuSize,int messageid)
        {
            List<TransportPacket> tPackets = new List<TransportPacket>();
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
            {
                _transport.Dispose();
                _heartbeatCancelToken.Cancel();
            }

            isDisposed = true;
        }
    }
}

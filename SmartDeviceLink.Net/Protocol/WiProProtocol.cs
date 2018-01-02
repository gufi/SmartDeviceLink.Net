using System;
using System.Collections.Generic;
using System.Text;
using SmartDeviceLink.Net.Converters;
using SmartDeviceLink.Net.Logging;
using SmartDeviceLink.Net.Protocol.Enums;
using SmartDeviceLink.Net.Transport;
using SmartDeviceLink.Net.Transport.Enums;

namespace SmartDeviceLink.Net.Protocol
{
    public class WiProProtocol
    {

        private ILogger _logger => Logger.SdlLogger;
        public static readonly int V1_V2_MTU_SIZE = 1500;
        public static readonly int V3_V4_MTU_SIZE = 131072;
        public static readonly int V1_HEADER_SIZE = 8;
        public static readonly int V2_HEADER_SIZE = 12;

        private int _messageId = 1;
        
        //TODO: Find where this is being set
        private int protocolVersion = 1;// this will eventually be set upon  start session

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
            message.Version = protocolVersion;
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
            transportPacket.ServiceType =  protocolPacket.SessionType;
            transportPacket.SessionId = protocolPacket.SessionId;
            transportPacket.DataSize = protocolPacketBytes?.Length ?? 0;
            transportPacket.Payload = protocolPacketBytes;
            transportPacket.ControlFrameInfo = FrameInfo.Heartbeat_FinalConsecutiveFrame_Reserved;
            transportPacket.PriorityCoefficient = protocolPacket.PriorityCoefficient;
            return transportPacket;
        }
    }
}

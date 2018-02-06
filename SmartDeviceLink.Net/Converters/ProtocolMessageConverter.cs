using System;
using System.Linq;
using SmartDeviceLink.Net.Protocol;
using SmartDeviceLink.Net.Protocol.Enums;
using SmartDeviceLink.Net.Protocol.Models;
using SmartDeviceLink.Net.Rpc.Base;
using SmartDeviceLink.Net.Transport;

namespace SmartDeviceLink.Net.Converters
{
    public static class ProtocolMessageConverter {
        /// <summary>
        /// First 4 bits are RpcType, 24 Function ID, 32 payload+bulkdata size, 32 correlation id, followed by data
        /// </summary>
        /// <returns></returns>
        public static byte[] ToProtocolFrame(this OutgoingProtocolPacket message)
        {
            byte[] packetbytes = null;
            var headersize = 8; // v1's at minimum have 8 bytes
            if (message.Version > 1) headersize += 4;

            var jsonSize = message.Payload?.Length ?? 0;
            var bulkdataSize = message.BulkData?.Length ?? 0;

            packetbytes = new byte[headersize + jsonSize + bulkdataSize];

            int firstBytes = (int) message.FunctionId;
            firstBytes &= 0xFFFFFFF;
            firstBytes |= message.RpcType << 24;
            Array.ConstrainedCopy(BitConverter.GetBytes(firstBytes.ReverseBytes()), 0, packetbytes, 0, 4);
            Array.ConstrainedCopy(BitConverter.GetBytes(jsonSize.ReverseBytes()), 0, packetbytes, 8, 4);
            if (message.Version > 1)
                Array.ConstrainedCopy(BitConverter.GetBytes(message.CorrelationId.ReverseBytes()), 0, packetbytes, 4, 4);
            if (message.Payload != null)
                Array.ConstrainedCopy(message.Payload, 0, packetbytes, headersize, jsonSize);
            if (message.BulkData != null)
                Array.ConstrainedCopy(message.BulkData, 0, packetbytes, headersize + jsonSize, bulkdataSize);
            return packetbytes;

        }

        public static ProtocolMessage ToProtocolMessage( this TransportPacket protocolFrame)
        {
            var message = new ProtocolMessage();
            var payload = protocolFrame.Payload;
            message.RpcType = (byte)((payload[0] >> 4).ReverseBytes());
            message.FunctionId = (FunctionID)(BitConverter.ToInt32(payload, 0).ReverseBytes() & 0xFFFFFFF);
            message.SessionId = protocolFrame.SessionId;
            message.ServiceType = protocolFrame.ServiceType;
            message.MessageId = protocolFrame.MessageId;
            //message.MessageType
            var jsonSize = BitConverter.ToInt32(payload, 8).ReverseBytes();
            if (protocolFrame.Version > 1)
            {
                message.CorrelationId = BitConverter.ToInt32(payload,4);
            }

            var startbyte = 8;
            if (protocolFrame.Version > 1)
            {
                startbyte += 4;
            }

            if (jsonSize > 0)
            {
                var jsonBytes = new byte[jsonSize];
                var test = System.Text.Encoding.ASCII.GetString((payload).Skip(13).ToArray());
                Array.ConstrainedCopy(payload, startbyte, jsonBytes, 0, jsonSize);
                message.Payload = jsonBytes;
            }

            if (payload.Length > startbyte + jsonSize)
            {
                var bulkdatalen = payload.Length - startbyte - jsonSize;
                var bulkdata = new byte[bulkdatalen];
                Array.ConstrainedCopy(payload, startbyte + jsonSize, bulkdata, 0, bulkdatalen);
                message.Payload = bulkdata;
            }

            return message;
        }

        private static int ReverseBytes(this int value)
        {
            return (int)((value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
                   (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24);
        }
    }
}
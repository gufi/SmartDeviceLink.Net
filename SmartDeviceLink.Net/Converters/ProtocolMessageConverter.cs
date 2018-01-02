using System;
using SmartDeviceLink.Net.Protocol;
using SmartDeviceLink.Net.Transport;

namespace SmartDeviceLink.Net.Converters
{
    public static class ProtocolMessageConverter {
        /// <summary>
        /// First 4 bits are RpcType, 24 Function ID, 32 payload+bulkdata size, 32 correlation id, followed by data
        /// </summary>
        /// <returns></returns>
        public static byte[] ToBytes(this OutgoingProtocolPacket message)
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
            Array.ConstrainedCopy(BitConverter.GetBytes(firstBytes), 0, packetbytes, 0, 4);
            Array.ConstrainedCopy(BitConverter.GetBytes(jsonSize), 0, packetbytes, 4, 4);
            if (message.Version > 1)
                Array.ConstrainedCopy(BitConverter.GetBytes(message.CorrelationId), 0, packetbytes, 8, 4);
            if (message.Payload != null)
                Array.ConstrainedCopy(message.Payload, 0, packetbytes, headersize, jsonSize);
            if (message.BulkData != null)
                Array.ConstrainedCopy(message.BulkData, 0, packetbytes, headersize + jsonSize, bulkdataSize);
            return packetbytes;

        }
    }
}
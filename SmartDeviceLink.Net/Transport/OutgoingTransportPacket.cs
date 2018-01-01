using System;
using System.Collections.Generic;
using System.Text;
using SmartDeviceLink.Net.Protocol.Enums;
using SmartDeviceLink.Net.Transport.Enums;

namespace SmartDeviceLink.Net.Transport
{
    public class OutgoingTransportPacket // replicates sdl_android SdlPacket
    {
        public int Version { get; set; }
        public bool Encryption { get; set; }

        // function id 4 bytes
        public FunctionID FunctionId { get; set; }
        public byte SessionId { get; set; }
        public int CorrelationId { get; set; }
        public int RpcType { get; set; }
        public byte[] Payload { get; set; }
        public byte[] BulkData { get; set; }

        /// <summary>
        /// First 4 bits are RpcType, 24 Function ID, 32 payload+bulkdata size, 32 correlation id, followed by data
        /// </summary>
        /// <returns></returns>
        public byte[] ToBytes()
        {
            byte[] packetbytes;
            var headersize = 8;// v1's at minimum have 8 bytes
            if (Version > 1) headersize += 4;

            var jsonSize = Payload?.Length ?? 0;
            var bulkdataSize = BulkData?.Length ?? 0;

            packetbytes = new byte[headersize + jsonSize + bulkdataSize];

            int firstBytes = (int) FunctionId;
            firstBytes &= 0xFFFFFFF;
            firstBytes |= RpcType << 24;
            Array.ConstrainedCopy(BitConverter.GetBytes(firstBytes), 0, packetbytes, 0, 4);
            Array.ConstrainedCopy(BitConverter.GetBytes(jsonSize),0,packetbytes,4,4);
            if(Version > 1)
                Array.ConstrainedCopy(BitConverter.GetBytes(CorrelationId), 0, packetbytes, 8, 4);
            if(Payload != null)
                Array.ConstrainedCopy(Payload, 0, packetbytes, headersize, jsonSize);
            if (BulkData != null)
                Array.ConstrainedCopy(BulkData, 0, packetbytes, headersize + jsonSize, bulkdataSize);
            return packetbytes;
        }
        
    }
}

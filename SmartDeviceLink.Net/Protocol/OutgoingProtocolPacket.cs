using System;
using System.Collections.Generic;
using System.Text;
using SmartDeviceLink.Net.Protocol.Enums;
using SmartDeviceLink.Net.Transport.Enums;

namespace SmartDeviceLink.Net.Transport
{
    public class OutgoingProtocolPacket // replicates sdl_android SdlPacket
    {
        public int Version { get; set; }
        // function id 4 bytes
        public FunctionID FunctionId { get; set; }
        public byte SessionId { get; set; }
        public int CorrelationId { get; set; }
        public int RpcType { get; set; }
        public byte[] Payload { get; set; }
        public byte[] BulkData { get; set; }

        
        
    }
}

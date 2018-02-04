using SmartDeviceLink.Net.Rpc.Base;

namespace SmartDeviceLink.Net.Protocol.Models
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

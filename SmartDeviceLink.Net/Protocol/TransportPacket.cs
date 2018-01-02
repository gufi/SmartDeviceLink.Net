using SmartDeviceLink.Net.Protocol.Enums;
using SmartDeviceLink.Net.Transport.Enums;

namespace SmartDeviceLink.Net.Protocol
{
    public class TransportPacket // replicates sdl_android SdlPacket
    {
        public int Version { get; set; }
        public FrameType FrameType { get; set; }
        public FrameInfo ControlFrameInfo { get; set; }
        public SessionType ServiceType { get; set; }
        public bool IsEncrypted { get; set; }
        public byte SessionId { get; set; }
        public int MessageId { get; set; }
        public int DataSize { get; set; }
        public int DumpSize { get; set; }
        public int PriorityCoefficient { get; set; }
        public byte[] Payload { get; set; }

       
    }
}

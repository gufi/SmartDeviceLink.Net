using SmartDeviceLink.Net.Protocol.Enums;
using SmartDeviceLink.Net.Transport;

namespace SmartDeviceLink.Net.Protocol.Models
{
    public class ProtocolMessage : OutgoingProtocolPacket
    {
        public ServiceType ServiceType { get; set; }
        public int JsonSize => Payload?.Length ?? 0;
        public bool IsPayloadProtected { get; set; }
        public int PriorityCoefficient { get; set; }
    }
}

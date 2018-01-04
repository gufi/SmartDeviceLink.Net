using System;
using System.Collections.Generic;
using System.Text;
using SmartDeviceLink.Net.Protocol.Enums;
using SmartDeviceLink.Net.Transport;

namespace SmartDeviceLink.Net.Protocol
{
    public class ProtocolMessage : OutgoingProtocolPacket
    {
        public MessageType MessageType { get; set; }
        public ServiceType ServiceType { get; set; }
        public int JsonSize => Payload?.Length ?? 0;
        public bool IsPayloadProtected { get; set; }
        public int PriorityCoefficient { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using SmartDeviceLink.Net.Transport.Enums;

namespace SmartDeviceLink.Net.Transport
{
    public class IncomingTransportPacket // replicates sdl_android SdlPacket
    {
        public int Version { get; set; }
        public bool Encryption { get; set; }
        public FrameType FrameType { get; set; }
        public FrameInfo ControlFrameInfo { get; set; }
        public byte ServiceType { get; set; }
        public int FrameInfo { get; set; }
        public byte SessionId { get; set; }
        public int MessageId { get; set; }
        public int DataSize { get; set; }
        public int DumpSize { get; set; }
        public int PriorityCoefficient { get; set; }
        public byte[] Payload { get; set; }

       
    }
}

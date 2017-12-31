using System;
using System.Collections.Generic;
using System.Text;
using SmartDeviceLink.Net.Protocol.Enums;

namespace SmartDeviceLink.Net.Protocol.Heartbeat
{
    public class ProtocolMessage
    {
        public byte Version { get; set; }
        public byte SessionID { get; set; }
        public byte[] Data { get; set; }
        public byte[] BulkData { get; set; }
        //public MessageType MessageType { get; set; }
        //public SessionType SessionType { get; set; }
        public byte RPCType { get; set; }
        public FunctionID FunctionID { get; set; }
        public int JsonSize => Data?.Length ?? 0;
        public bool PayloadProtected { get; set; }
        public int PriorityCoefficient { get; set; }
    }
}

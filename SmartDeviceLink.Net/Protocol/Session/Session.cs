using System;
using System.Collections.Generic;
using System.Text;
using SmartDeviceLink.Net.Protocol.Enums;

namespace SmartDeviceLink.Net.Protocol.Session
{
    public class Session
    {
        public SessionType SessionType { get; set; }
        public byte SessionId { get; set; }

    }
}

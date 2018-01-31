using System;
using System.Collections.Generic;
using System.Text;
using SmartDeviceLink.Net.Protocol.Enums;

namespace SmartDeviceLink.Net.Protocol.Session
{
    /// <summary>
    /// Sessions are Service types and related data.
    /// This class represents an active session
    /// </summary>
    public class Session
    {
        public ServiceType ServiceType { get; set; }
        public byte SessionId { get; set; }
        public int  Version { get; set; }
        public DateTime LastTxRx { get; set; }
    }
}

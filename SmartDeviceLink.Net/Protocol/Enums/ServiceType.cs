using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace SmartDeviceLink.Net.Protocol.Enums
{
    public enum ServiceType : byte
    {
        Control = 0,
        Rpc = 0x7,
        Pcm = 0xA,
        Nav = 0xB,
        BulkData = 0xF
    }
}

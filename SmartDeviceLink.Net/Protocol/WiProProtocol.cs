using System;
using System.Collections.Generic;
using System.Text;

namespace SmartDeviceLink.Net.Protocol
{
    public class WiProProtocol
    {
        public static readonly int V1_V2_MTU_SIZE = 1500;
        public static readonly int V3_V4_MTU_SIZE = 131072;
        public static readonly int V1_HEADER_SIZE = 8;
        public static readonly int V2_HEADER_SIZE = 12;
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace SmartDeviceLink.Net.Transport.Enums
{
    public enum FrameType : byte
    {
        TypeControl,
        Single,
        First,
        Consecutive
    }
}

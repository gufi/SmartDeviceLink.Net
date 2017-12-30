using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDeviceLink.Net.Rpc.Base;

namespace SmartDeviceLink.Net.SdlService
{
    public class SdlMessageManager
    {

        public void HandleMessage(byte[] messagePayload)
        {
            var jsonPayload = new string(messagePayload.Select(x =>(char)x).ToArray());
        }
    }
}

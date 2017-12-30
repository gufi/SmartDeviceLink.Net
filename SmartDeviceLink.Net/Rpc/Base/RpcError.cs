using System;
using System.Collections.Generic;
using System.Text;

namespace SmartDeviceLink.Net.Rpc.Base
{
    public class RpcError
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public Dictionary<string,string> Data { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace SmartDeviceLink.Net.Rpc.Base
{
    public class RPCMessageResponse<T> 
    {

        public int Id { get; set; }
        public string JsonRpc { get; set; }
        public T Result { get; set; }
        public RpcError Error { get; set; }

    }
}

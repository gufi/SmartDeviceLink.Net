using System;
using System.Collections.Generic;
using System.Text;

namespace SmartDeviceLink.Net.Rpc.Base
{
    public class RpcRequest<T> : RpcRequest where T : new()
    {
        public RpcRequest() => Params = new T();
        public T Params { get; set; }
    }

    public class RpcRequest
    {
        public int Id { get; set; }
        public int AppId { get; set; }
        public string JsonRpc { get; set; }
        public string Method { get; set; }
    }
}

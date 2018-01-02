using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SmartDeviceLink.Net.Protocol.Enums;

namespace SmartDeviceLink.Net.Rpc.Base
{
    public class RpcRequest<T> : RpcRequest where T : new()
    {
        public RpcRequest() => Params = new T();
        public T Params { get; set; }
    }

    public class RpcRequest
    {
        [JsonIgnore]
        public FunctionID Id { get; set; }
        public string FunctionName { get { return Id.ToString(); } }
        public int FunctionId
        {
            get { return (int)Id; }
        }
        public int AppId { get; set; }
        public string JsonRpc { get; set; }
        public string Method { get; set; }
        public bool IsPayloadProtected { get; set; }
        public int CorrelationId { get; set; }

        [JsonIgnore]
        public byte[] BulkData { get; set; }

    }
}

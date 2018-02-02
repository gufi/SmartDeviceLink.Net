using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SmartDeviceLink.Net.Protocol.Enums;

namespace SmartDeviceLink.Net.Rpc.Base
{
    public abstract class RpcRequestWithBulkData<T> : RpcRequest<T> where T : new()
    {
        public byte[] bulkData { get; set; }
    }
    public abstract class RpcRequest<T> : RpcRequest where T : new()
    {
        public RpcRequest() => parameters = new T();
        public T parameters { get; set; }
    }

    public abstract class RpcRequest
    {
        [JsonIgnore]
        public abstract FunctionID FunctionId { get; set; } 
        public string name
        {
            get => FunctionId.ToString();
            set => FunctionId = (FunctionID)Enum.Parse(typeof(FunctionID),value);
        }
        

        // ReSharper disable once InconsistentNaming
        public int correlationID { get; set; } = 1;
        [JsonIgnore]
        public bool IsPayloadProtected { get; set; }

    }
}

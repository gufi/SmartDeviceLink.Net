using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SmartDeviceLink.Net.Protocol;
using SmartDeviceLink.Net.Protocol.Enums;
using SmartDeviceLink.Net.Protocol.Models;
using SmartDeviceLink.Net.Rpc.Base;

namespace SmartDeviceLink.Net.Converters
{
    public static class RpcRequestConverter
    {
        private static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            Converters = new List<JsonConverter>(new List<JsonConverter>()),
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        //todo: convert to interface
        public static ProtocolMessage ToProtocolMessage(this RpcRequest request)
        {
            var protocolMessage = new ProtocolMessage();

            protocolMessage.Payload = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(request, _jsonSettings));
            protocolMessage.ServiceType = ServiceType.Rpc;
            protocolMessage.FunctionId = request.FunctionId;
            protocolMessage.IsPayloadProtected = request.IsPayloadProtected;
            protocolMessage.CorrelationId = request.CorrelationID;
            //protocolMessage.BulkData = request.BulkData;
            if (request.FunctionId.Equals(FunctionID.PutFile))
                protocolMessage.PriorityCoefficient = 1;
            return protocolMessage;
        }
    }
}
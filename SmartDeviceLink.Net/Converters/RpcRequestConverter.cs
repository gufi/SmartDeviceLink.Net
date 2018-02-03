using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using SmartDeviceLink.Net.Protocol;
using SmartDeviceLink.Net.Protocol.Enums;
using SmartDeviceLink.Net.Rpc.Base;

namespace SmartDeviceLink.Net.Converters
{
    public static class RpcRequestConverter
    {
        private static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            Converters = new List<JsonConverter>(new List<JsonConverter>())
        };
        //todo: convert to interface
        public static ProtocolMessage ToProtocolMessage(this RpcRequest request)
        {
            var protocolMessage = new ProtocolMessage();

            protocolMessage.Payload = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(request, _jsonSettings));
            protocolMessage.MessageType = MessageType.Rpc;
            protocolMessage.ServiceType = ServiceType.Rpc;
            protocolMessage.FunctionId = request.FunctionId;
            protocolMessage.IsPayloadProtected = request.IsPayloadProtected;
            protocolMessage.CorrelationId = request.correlationID;
            //protocolMessage.BulkData = request.BulkData;
            if (request.FunctionId.Equals(FunctionID.PutFile))
                protocolMessage.PriorityCoefficient = 1;
            return protocolMessage;
        }


    }

    public class BoolConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((bool)value) ? "true" : "false");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return reader.Value.ToString().Equals("true",StringComparison.CurrentCultureIgnoreCase);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(bool);
        }
    }
}
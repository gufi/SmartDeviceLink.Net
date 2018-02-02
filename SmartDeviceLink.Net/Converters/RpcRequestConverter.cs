using System;
using System.Text;
using Newtonsoft.Json;
using SmartDeviceLink.Net.Protocol;
using SmartDeviceLink.Net.Protocol.Enums;
using SmartDeviceLink.Net.Rpc.Base;

namespace SmartDeviceLink.Net.Converters
{
    public static class RpcRequestConverter
    {
        //todo: convert to interface
        public static ProtocolMessage ToProtocolMessage(this RpcRequest request)
        {
            var protocolMessage = new ProtocolMessage();

            protocolMessage.Payload = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(request,Formatting.None,new BoolConverter()));
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
            writer.WriteValue(((bool)value) ? 1 : 0);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return reader.Value.ToString() == "1";
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(bool);
        }
    }
}
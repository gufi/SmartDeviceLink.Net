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

            protocolMessage.Payload = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(request));
            protocolMessage.MessageType = MessageType.Rpc;
            protocolMessage.ServiceType = ServiceType.Rpc;
            protocolMessage.FunctionId = request.Id;
            protocolMessage.IsPayloadProtected = request.IsPayloadProtected;
            protocolMessage.CorrelationId = request.CorrelationId;
            protocolMessage.BulkData = request.BulkData;
            if (request.Id.Equals(FunctionID.PutFile))
                protocolMessage.PriorityCoefficient = 1;
            return protocolMessage;
        }
    }
}
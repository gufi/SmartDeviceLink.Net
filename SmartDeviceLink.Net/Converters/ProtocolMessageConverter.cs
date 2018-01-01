using SmartDeviceLink.Net.Protocol;
using SmartDeviceLink.Net.Transport;

namespace SmartDeviceLink.Net.Converters
{
    public static class ProtocolMessageConverter
    {
        //todo convert to interface
        public static OutgoingTransportPacket ToOutgoingTransportPacket(this ProtocolMessage message)
        {
            var packet = new OutgoingTransportPacket();
            packet.Payload = message.Data;
            packet.BulkData = message.BulkData;
            packet.CorrelationId = message.CorrelationId;
            packet.FunctionId = message.FunctionID;
            packet.RpcType = message.RPCType;
            packet.SessionId = message.SessionID;
            packet.Version = message.Version;
            return packet;
        }
    }
}
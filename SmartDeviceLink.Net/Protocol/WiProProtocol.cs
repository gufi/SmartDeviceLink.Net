using System;
using System.Collections.Generic;
using System.Text;
using SmartDeviceLink.Net.Protocol.Enums;
using SmartDeviceLink.Net.Transport;

namespace SmartDeviceLink.Net.Protocol
{
    public class WiProProtocol
    {
        public static readonly int V1_V2_MTU_SIZE = 1500;
        public static readonly int V3_V4_MTU_SIZE = 131072;
        public static readonly int V1_HEADER_SIZE = 8;
        public static readonly int V2_HEADER_SIZE = 12;

        
        //TODO: Find where this is being set
        private int protocolVersion = 2;

        public OutgoingTransportPacket CreateTransportPacket(ProtocolMessage message)
        {
            message.RPCType = 0; // feel like this needs to be some kind of enum later
            var transportPacket = new OutgoingTransportPacket();
            List<Byte> databuffer = null;
            if (protocolVersion > 1 && message.SessionType != SessionType.Pcm && message.SessionType != SessionType.Nav)
            {
                if (message.SessionType == SessionType.Control)
                {
                    //databuffer = new List<Byte>(V1_HEADER_SIZE + message.JsonSize + message.BulkData.Length);
                    transportPacket.Version = 1;
                }
                else if (message.BulkData != null) // Pcm or Nav type packet
                {
                    databuffer = new List<Byte>(V2_HEADER_SIZE + message.JsonSize + message.BulkData.Length);
                    message.SessionType = SessionType.BulkData;
                }
                else
                {
                    databuffer = new List<Byte>(V2_HEADER_SIZE + message.JsonSize);
                }
            }

            if (databuffer != null) message.Data = databuffer.ToArray();
        }
    }
}

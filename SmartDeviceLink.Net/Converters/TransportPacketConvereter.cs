using SmartDeviceLink.Net.Protocol;
using SmartDeviceLink.Net.Protocol.Enums;
using SmartDeviceLink.Net.Transport;
using SmartDeviceLink.Net.Transport.Enums;

namespace SmartDeviceLink.Net.Converters
{
    public static class TransportPacketConvereter
    {
        public static byte[] ToBytes(this TransportPacket packet)
        {
            return GeneratePacket(
                packet.Version, 
                packet.FrameType, 
                packet.ServiceType,
                packet.ControlFrameInfo,
                packet.SessionId, 
                packet.MessageId,
                packet.Payload);
        }

        private static byte[] GeneratePacket(int version, FrameType frameType, SessionType serviceType, FrameInfo frameInfoType,
            byte sessionId, int message, byte[] databytes)
        {
            var dataSize = 0;
            if (databytes != null)
            {
                dataSize += databytes.Length;
            }
            dataSize += 4 + // data size
                        1 + //version + frameType
                        1 + // service Type
                        1 + // frameInfo Type
                        1; //sessionId
            if (version > 1) dataSize += 4; // message   
            var packetBytes = new byte[dataSize];
            byte versionAndFrameType = (byte)((version << 4) | (byte)frameType);
            packetBytes[0] = versionAndFrameType;
            packetBytes[1] = (byte)serviceType;
            packetBytes[2] = (byte)frameInfoType;
            packetBytes[3] = sessionId;

            if (databytes != null)
            {
                packetBytes[4] = (byte)(databytes.Length >> 24);
                packetBytes[5] = (byte)(databytes.Length >> 16);
                packetBytes[6] = (byte)(databytes.Length >> 8);
                packetBytes[7] = (byte)databytes.Length;
            }
            var byteI = 8;

            if (version != 1)
            {
                packetBytes[byteI] = (byte)(message >> 24); byteI++;
                packetBytes[byteI] = (byte)(message >> 16); byteI++;
                packetBytes[byteI] = (byte)(message >> 8); byteI++;
                packetBytes[byteI] = (byte)message; byteI++;
            }
            if (databytes != null)
            {
                for (int i = 0; i < databytes.Length; i++)
                {
                    packetBytes[byteI + i] = databytes[i];
                }
            }
            return packetBytes;
        }
    }
}
using System;
using SmartDeviceLink.Net.Logging;
using SmartDeviceLink.Net.Protocol.Enums;
using SmartDeviceLink.Net.Transport;
using SmartDeviceLink.Net.Transport.Enums;
using SmartDeviceLink.Net.Transport.Interfaces;

namespace SmartDeviceLink.Net.Protocol
{
    /// <summary>
    /// ProtocolPacketParser is a State Machine
    /// as each byte comes in the state is upgraded based on the byte being processed
    /// each current state is defined by the ByteHandler property
    /// once a packet is considered completed HandlePacket() is called which calls the passed in action to the ctor
    /// </summary>
    public class ProtocolPacketParser : IPacketParser
    {
        private ILogger _logger => Logger.SdlLogger;
        private readonly Action<TransportPacket> _packetHandler;
        private static readonly byte FIRST_FRAME_DATA_SIZE = 0x08;
        private static readonly int VERSION_MASK = 0xF0; //4 highest bits
        private static readonly int COMPRESSION_MASK = 0x08; //4th lowest bit
        private static readonly int FRAME_TYPE_MASK = 0x07; //3 lowest bits

        private static readonly int V1_V2_MTU_SIZE = WiProProtocol.V1_V2_MTU_SIZE;
        private static readonly int V3_V4_MTU_SIZE = WiProProtocol.V3_V4_MTU_SIZE;
        private static readonly int V1_HEADER_SIZE = WiProProtocol.V1_HEADER_SIZE;
        private static readonly int V2_HEADER_SIZE = WiProProtocol.V2_HEADER_SIZE;

        private TransportPacket packet;
        public ProtocolPacketParser(Action<TransportPacket> packetHandler)
        {
            _packetHandler = packetHandler;
            ByteHandler = StartState;
        }
        private Action<byte> ByteHandler { get; set; }

        public void HandleByte(byte data)
        {
            ByteHandler(data);
        }

        private void HandlePacket()
        {
            _packetHandler(packet);
            ByteHandler = StartState;
        }

        protected virtual void StartState(byte data)
        {
            packet = new TransportPacket();
            packet.Version = (data & VERSION_MASK) >> 4;
            if (packet.Version < 1 || packet.Version > 5)
            {
                _logger.LogError("Packet Version Invalid" + packet.Version);
                return;
            }
            packet.IsEncrypted = (data & COMPRESSION_MASK) > 0; // sdl_android doesnt use this?
            var frameType = (byte)(data & FRAME_TYPE_MASK);
            if (!Enum.IsDefined(typeof(FrameType), frameType))
            {
                _logger.LogError("FrameType undefined " + frameType);
                return;
            }
            packet.FrameType = (FrameType)frameType;
            _logger.LogVerbose($"Version: {packet.Version} Frame Type: { packet.FrameType}");
            ByteHandler = ServiceTypeState;
        }

        protected virtual void ServiceTypeState(byte data)
        {
            packet.ServiceType = (SessionType)(data & 0xFF);
            ByteHandler = ControlFrameInfoState;
            _logger.LogVerbose($"Service Type: {packet.ServiceType}");
        }

        protected virtual void ControlFrameInfoState(byte data)
        {
            switch (packet.FrameType)
            {
                case FrameType.Consecutive:
                    break;
                case FrameType.Control:
                    break;
                case FrameType.First:
                case FrameType.Single:
                    {
                        if ((data & 0xFF) > 0)
                        {
                            ByteHandler = StartState; // must be FrameInfo.HeatBeat_FinalConsecutiveFrame_Reserved
                            return;
                        }

                    }
                    break;
                default:
                    {// due to enum i dont think this can ever be hit
                        ByteHandler = StartState;
                        return;
                    }
            }
            if (Enum.IsDefined(typeof(FrameInfo), data)) packet.ControlFrameInfo = (FrameInfo)(int)data;
            else
            {
                ByteHandler = StartState;
                _logger.LogError("Invalid Frame Info Type: " + data);
                return;
            }
            ByteHandler = SessionIdState;
        }

        protected virtual void SessionIdState(byte data)
        {
            packet.SessionId = (byte)(data & 0xFF);
            ByteHandler = DataSize1State;
        }

        protected virtual void DataSize1State(byte data)
        {
            packet.DataSize |= data << 24;
            ByteHandler = DataSize2State;
        }

        protected virtual void DataSize2State(byte data)
        {
            packet.DataSize |= data << 16;
            ByteHandler = DataSize3State;
        }

        protected virtual void DataSize3State(byte data)
        {
            packet.DataSize |= data << 8;
            ByteHandler = DataSize4State;
        }

        protected virtual void DataSize4State(byte data)
        {
            packet.DataSize |= data;
            _logger.LogVerbose("DataSize: " + packet.DataSize);
            switch (packet.FrameType)
            {
                case FrameType.Consecutive:
                case FrameType.Single:
                case FrameType.Control:
                    break;
                case FrameType.First:
                    {
                        if (packet.DataSize != FIRST_FRAME_DATA_SIZE) ByteHandler = StartState;// this is an error state, so start over
                    }
                    break;
                default:
                    ByteHandler = StartState;
                    break;
            }

            if (packet.Version == 1)
            {
                if (packet.DataSize == 0)
                {
                    HandlePacket();
                    return;
                }
                if (packet.DataSize <= V1_V2_MTU_SIZE - V1_HEADER_SIZE)
                {
                    packet.Payload = new byte[packet.DataSize];
                }
                else
                {
                    ByteHandler = StartState;
                    return;
                }
                ByteHandler = DataPumpState;
            }
            else // non version 1 packet
            {
                packet.Payload = new byte[packet.DataSize];
                ByteHandler = Message1State;
            }
        }

        protected virtual void Message1State(byte data)
        {
            packet.MessageId = data << 24;
            ByteHandler = Message2State;
        }
        protected virtual void Message2State(byte data)
        {
            packet.MessageId |= data << 16;
            ByteHandler = Message3State;
        }
        protected virtual void Message3State(byte data)
        {
            packet.MessageId |= data << 8;
            ByteHandler = Message4State;
        }
        protected virtual void Message4State(byte data)
        {
            packet.MessageId |= data;
            if (packet.DataSize == 0)
            {
                HandlePacket();
                return;
            }
            // sdl_android creates payload buffer here, but that was handled in data size 4
            ByteHandler = DataPumpState;
            _logger.LogVerbose("MessageId: " + packet.MessageId);
        }

        protected virtual void DataPumpState(byte data)
        {
            if (packet.DumpSize < packet.Payload.Length)
            {
                packet.Payload[packet.DumpSize++] = data;
                _logger.LogVerbose($"Recieved Byte {packet.DumpSize} of {packet.Payload.Length}");
                if(packet.DumpSize+1 == packet.Payload.Length)
                    HandlePacket();
            }
        }
    }
}

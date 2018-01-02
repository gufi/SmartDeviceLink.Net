using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartDeviceLink.Net.Protocol;
using SmartDeviceLink.Net.Transport;

namespace SmartDeviceLink.Net.UnitTests.Transport
{
    public class ProxyProtocolPacketParser : ProtocolPacketParser
    {
        public ProxyProtocolPacketParser(Action<TransportPacket> packetHandler) : base(packetHandler)
        {
        }

        public int ControlFrameInfoStateCount { get; set; }
        protected override void ControlFrameInfoState(byte data)
        {
            ControlFrameInfoStateCount++;
            base.ControlFrameInfoState(data);
        }

        public int DataPumpStateCount { get; set; }
        protected override void DataPumpState(byte data)
        {
            DataPumpStateCount++;
            base.DataPumpState(data);
        }

        public int DataSize1Count { get; set; }
        protected override void DataSize1State(byte data)
        {
            DataSize1Count++;
            base.DataSize1State(data);
        }

        public int DataSize2Count { get; set; }
        protected override void DataSize2State(byte data)
        {
            DataSize2Count++;
            base.DataSize2State(data);
        }

        public int DataSize3Count { get; set; }
        protected override void DataSize3State(byte data)
        {
            DataSize3Count++;
            base.DataSize3State(data);
        }

        public int DataSize4Count { get; set; }
        protected override void DataSize4State(byte data)
        {
            DataSize4Count++;
            base.DataSize4State(data);
        }

        public int MessageSize1Count { get; set; }
        protected override void Message1State(byte data)
        {
            MessageSize1Count++;
            base.Message1State(data);
        }
        public int MessageSize2Count { get; set; }
        protected override void Message2State(byte data)
        {
            MessageSize2Count++;
            base.Message2State(data);
        }
        public int MessageSize3Count { get; set; }
        protected override void Message3State(byte data)
        {
            MessageSize4Count++;
            base.Message3State(data);
        }
        public int MessageSize4Count { get; set; }
        protected override void Message4State(byte data)
        {
            MessageSize4Count++;
            base.Message4State(data);
        }

        public int ServiceTypeCount { get; set; }
        protected override void ServiceTypeState(byte data)
        {
            ServiceTypeCount++;
            base.ServiceTypeState(data);
        }

        public int SessionIdCount { get; set; }
        protected override void SessionIdState(byte data)
        {
            SessionIdCount++;
            base.SessionIdState(data);
        }

        public int StartCount { get; set; }
        protected override void StartState(byte data)
        {
            StartCount++;
            base.StartState(data);
        }
        
        
    }
}

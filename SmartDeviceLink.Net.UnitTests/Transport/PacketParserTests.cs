using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SmartDeviceLink.Net.Protocol;
using SmartDeviceLink.Net.Transport;
using SmartDeviceLink.Net.Transport.Enums;
using SmartDeviceLink.Net.Transport.Interfaces;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace SmartDeviceLink.Net.UnitTests.Transport
{
    [TestFixture]
    public class PacketParserTests
    {
        private IPacketParser _parser;
        private List<TransportPacket> _packets;
        [SetUp]
        public void TestInit()
        {
            _parser = new ProxyProtocolPacketParser((packet) => _packets.Add(packet));
            _packets = new List<TransportPacket>();
        }

        [Test]
        public void FirstFrameV1PacketTest()
        {
            int version = 1;
            int frameType = (byte)FrameType.First;
            byte serviceType = 1;
            byte frameInfoType = 0;
            byte sessionId = 1;
            int message = 2;
            var data = Encoding.ASCII.GetBytes("HelloWor");
            RunParser(GeneratePacket(version, frameType, serviceType, frameInfoType, sessionId, message, data));

            Assert.IsTrue(_packets.Count == 1);
            var packet = _packets.First();
            AssertPacket(packet, version, frameType, serviceType, frameInfoType, data);
        }

        [Test]
        public void HelloWorldPacketTest()
        {
            int version = 1;
            int frameType = 1;
            byte serviceType = 1;
            byte frameInfoType = 0;
            byte sessionId = 1;
            int message = 2;
            var data = Encoding.ASCII.GetBytes("Hello World");
            RunParser(GeneratePacket(version,frameType,serviceType,frameInfoType,sessionId,message,data));
            
            Assert.IsTrue(_packets.Count == 1);
            var packet = _packets.First();
            AssertPacket(packet, version, frameType, serviceType, frameInfoType, data);
        }
        
        [Test]
        public void HelloWorldPacketTestv2()
        {
            int version = 2;
            int frameType = (byte)FrameType.Control;
            byte serviceType = 99;
            byte frameInfoType = (byte)FrameInfo.HeartBeatAck;
            byte sessionId = 100;
            int message = 200;
            var data = Encoding.ASCII.GetBytes("Hello World");
            RunParser(GeneratePacket(version, frameType, serviceType, frameInfoType, sessionId, message, data));

            Assert.IsTrue(_packets.Count == 1);
            var packet = _packets.First();
            AssertPacket(packet, version, frameType, serviceType, frameInfoType, data);
        }

        [Test]
        public void PacketTestNoData()
        {
            int version = 2;
            int frameType = (byte)FrameType.Control;
            byte serviceType = 99;
            byte frameInfoType = (byte)FrameInfo.HeartBeatAck;
            byte sessionId = 100;
            int message = 200;
            byte[] data = null;
            RunParser(GeneratePacket(version, frameType, serviceType, frameInfoType, sessionId, message, data));

            Assert.IsTrue(_packets.Count == 1);
            var packet = _packets.First();
            AssertPacket(packet, version, frameType, serviceType, frameInfoType, data);
        }

        [Test]
        public void MultiplePacketsSingleArray()
        {
            int frameType = (byte)FrameType.Control;
            byte serviceType = 99;
            byte frameInfoType = 0;
            byte sessionId = 100;
            int message = 200;
            var data = Encoding.ASCII.GetBytes("Hello World");
            List<byte> dataList = new List<byte>();
            for (int i = 0; i < 10; i++)
            {
                dataList.AddRange(GeneratePacket(i%2+1, frameType, serviceType, frameInfoType, sessionId, message,(i%3 == 0)? data:null));
            }

            RunParser(dataList.ToArray());

            Assert.IsTrue(_packets.Count == 10);
            Assert.AreEqual(_packets.Count(x => x.Version ==1),5);
            Assert.AreEqual(_packets.Count(x => x.DataSize > 0), 4);
        }

        [Test]
        public void PacketInvalid()
        {
            int version = 1;
            int frameType = (byte)FrameType.Single;
            byte serviceType = 99;
            byte frameInfoType = (byte)FrameInfo.HeartBeatAck;// frame info for single must be 0
            byte sessionId = 100;
            int message = 200;
            var data = Encoding.ASCII.GetBytes("Hello World");
            RunParser(GeneratePacket(version, frameType, serviceType, frameInfoType, sessionId, message, data));

            Assert.IsTrue(_packets.Count == 0);
        }


        private  void RunParser(byte[] bytes)
        {
            foreach(var b in bytes) _parser.HandleByte(b);
        }

        private static void AssertPacket(TransportPacket packet, int version, int frameType, byte serviceType,
            byte frameInfoType, byte[] data)
        {
            Assert.AreEqual(packet.Version, version);
            Assert.AreEqual((byte)packet.FrameType, frameType);
            Assert.AreEqual(packet.ServiceType, serviceType);
            Assert.AreEqual((byte)packet.ControlFrameInfo, frameInfoType);
            Assert.AreEqual(packet.DataSize, data?.Length ?? 0);
        }

        private static byte[] GeneratePacket(int version, int frameType, byte serviceType, byte frameInfoType,
            byte sessionId, int message, byte[] databytes)
        {
            var dataSize = 0;
            if (databytes != null)
            {
                dataSize += databytes.Length;
            }
            dataSize +=     4 + // data size
                            1 + //version + frameType
                            1 + // service Type
                            1 + // frameInfo Type
                            1; //sessionId
            if (version > 1) dataSize +=4; // message   
            var packetBytes = new byte[dataSize];
            byte versionAndFrameType = (byte) ((version << 4) | frameType);
            packetBytes[0] = versionAndFrameType;
            packetBytes[1] = serviceType;
            packetBytes[2] = frameInfoType;
            packetBytes[3] = sessionId;

            if (databytes != null)
            {
                packetBytes[4] = (byte) (databytes.Length >> 24);
                packetBytes[5] = (byte) (databytes.Length >> 16);
                packetBytes[6] = (byte) (databytes.Length >> 8);
                packetBytes[7] = (byte) databytes.Length;
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

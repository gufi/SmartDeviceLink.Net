using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SmartDeviceLink.Net.Transport;
using SmartDeviceLink.Net.Transport.Enums;
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
            _parser = new ProxyPacketParser((packet) => _packets.Add(packet));
            _packets = new List<TransportPacket>();
        }
        [Test]
        public void HelloWorldPacketTest()
        {
            var data = Encoding.ASCII.GetBytes("Hello World");
            RunParser(GeneratePacket(1,1,1,0,1,2,data));
            
            Assert.IsTrue(_packets.Count == 1);
            var packet = _packets.First();
            Assert.AreEqual(packet.Version,1);
            Assert.AreEqual((byte)packet.FrameType, 1);
            Assert.AreEqual((byte)packet.ControlFrameInfo, 0);
            Assert.AreEqual(packet.DataSize, data.Length);
        }

        private  void RunParser(byte[] bytes)
        {
            foreach(var b in bytes) _parser.HandleByte(b);
        }

        private static byte[] GeneratePacket(int version, int frameType, byte serviceType, byte frameInfoType,
            byte sessionId, int message, byte[] databytes)
        {
            var dataSize =
                databytes.Length +
                4 + // data size
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

            packetBytes[4] = (byte)(databytes.Length >> 24);
            packetBytes[5] = (byte)(databytes.Length >> 16);
            packetBytes[6] = (byte)(databytes.Length >> 8);
            packetBytes[7] = (byte)databytes.Length;

            var byteI = 8;

            if (version != 1)
            {
                packetBytes[byteI] = (byte)(message >> 24); byteI++;
                packetBytes[byteI] = (byte)(message >> 16); byteI++;
                packetBytes[byteI] = (byte)(message >> 8); byteI++;
                packetBytes[byteI] = (byte)message; byteI++;
            }

            for (int i = 0; i < databytes.Length; i++)
            {
                packetBytes[byteI + i] = databytes[i];
            }

            return packetBytes;
        }
    }
}

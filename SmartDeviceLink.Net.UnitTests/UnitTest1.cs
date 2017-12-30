using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartDeviceLink.Net.Transport;

namespace SmartDeviceLink.Net.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            //var transport = new TcpTransport("m.sdl.tools",5404);
            //await transport.ConnectAsync();
            //await transport.SendBytesAsync(
            //    Encoding.ASCII.GetBytes("{\r\n  \"id\" : 87,\r\n  \"jsonrpc\" : \"2.0\",\r\n  \"method\" : \"BasicCommunication.AllowDeviceToConnect\",\r\n  \"params\" :\r\n  [\r\n    \"deviceInfo\" : {\r\n        \"name\" : \"Mary`s Phone\",\r\n        \"id\" : 8\r\n    }\r\n  ]\r\n}"));
        }
    }
}

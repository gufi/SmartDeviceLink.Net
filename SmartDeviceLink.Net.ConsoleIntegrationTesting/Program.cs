using SmartDeviceLink.Net.Transport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartDeviceLink.Net.Bson;
using SmartDeviceLink.Net.Rpc.Base;
using SmartDeviceLink.Net.Rpc.BasicCommunication;

namespace SmartDeviceLink.Net.ConsoleIntegrationTesting
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }
        static async Task MainAsync(string[] args)
        {
            var packets = new List<IncomingTransportPacket>();
            using (var transport = new TcpTransport("m.sdl.tools", 5264, (packet) =>
            {
                packets.Add(packet);
                Console.WriteLine("packet recieved");
            }))
            {
                await transport.ConnectAsync();
                Console.WriteLine("Connected");
                var rpc = new RpcRequest<AllowDeviceToConnect>();
                rpc.Id = 87;
                rpc.AppId = 100;
                rpc.Method = "BasicCommunication.AllowDeviceToConnect";
                rpc.JsonRpc = "2.0";
                rpc.Params.DeviceInfo.Id = 9;
                rpc.Params.DeviceInfo.Name = "Test Phone";
                //await transport.SendAsync(Encoding.ASCII.GetBytes(BsonConvert.ToBson(rpc)));
                Console.ReadKey();
            }
        }
    }
}

using SmartDeviceLink.Net.Transport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartDeviceLink.Net.Bson;
using SmartDeviceLink.Net.Logging;
using SmartDeviceLink.Net.Protocol;
using SmartDeviceLink.Net.Protocol.Enums;
using SmartDeviceLink.Net.Rpc.Base;
using SmartDeviceLink.Net.Rpc.BasicCommunication;
using SmartDeviceLink.Net.SdlService;

namespace SmartDeviceLink.Net.ConsoleIntegrationTesting
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Logger.SdlLogger = new ConsoleLogger();
            MainAsync(args).Wait();
        }
        static async Task MainAsync(string[] args)
        {
            var packets = new List<TransportPacket>();
            using (var client = new SdlClient(new TcpTransport("m.sdl.tools", 5433)))
            {
                await client.StartSessionAsync(SessionType.Rpc);
                Console.WriteLine("Connected");
                var rpc = new RpcRequest<AllowDeviceToConnect>();
                rpc.Id = FunctionID.Show;
                rpc.AppId = 100;
                rpc.Method = "BasicCommunication.AllowDeviceToConnect";
                rpc.JsonRpc = "2.0";
                rpc.Params.DeviceInfo.Id = 9;
                rpc.Params.DeviceInfo.Name = "Test Phone";
                char exit = 'a';
                do
                {
                    await client.SendAsync(rpc);
                    exit = Console.ReadKey().KeyChar;
                } while (exit != 'e');
            }
        }
    }
}

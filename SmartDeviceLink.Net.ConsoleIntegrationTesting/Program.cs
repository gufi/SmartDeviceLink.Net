using SmartDeviceLink.Net.Transport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SmartDeviceLink.Net.Bson;
using SmartDeviceLink.Net.Logging;
using SmartDeviceLink.Net.Protocol;
using SmartDeviceLink.Net.Protocol.Enums;
using SmartDeviceLink.Net.Protocol.Models;
using SmartDeviceLink.Net.Rpc.Base;
using SmartDeviceLink.Net.Rpc.BasicCommunication;
using SmartDeviceLink.Net.Rpc.Common;
using SmartDeviceLink.Net.Rpc.Response;
using SmartDeviceLink.Net.SdlService;

namespace SmartDeviceLink.Net.ConsoleIntegrationTesting
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Logger.SdlLogger = new ConsoleLogger();
            Logger.SdlLogger.LogLevel = LogLevel.Info;
            var maintask = MainAsync(args);
            maintask.ConfigureAwait(false);
            maintask.Wait();

        }
        static async Task MainAsync(string[] args)
        {
            var packets = new List<TransportPacket>();
            using (var client = new SdlClient(new TcpTransport("m.sdl.tools", 5539)))
            {
                client.OnSystemRequest += request => Logger.SdlLogger.LogInfo("On System Request event triggered",request);
                client.OnHmiStatus += request => Logger.SdlLogger.LogInfo("On HMI status event triggered",request);
                Logger.SdlLogger.LogInfo("Connected");
                var rpc = new RegisterAppInterface();
                bool exit = false;
                await client.StartSession();
                await Task.Delay(5000);
                await client.RegisterAppWithHmi(rpc);


                Logger.SdlLogger.LogVerbose("Hmi Info", client.HmiInfo);

                do
                {
                    try
                    {
                        if (Console.KeyAvailable)
                        {
                            var key = Console.ReadKey().KeyChar;
                            if (key == 'c') Console.Clear();
                            else if (key == 'e') exit = true;
                            else if (key == 'r')
                            {
                                var data = await client.SendAsync<object>(new SubscribeVehicleData());
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                    await Task.Delay(100).ConfigureAwait(false);
                } while (exit != true);


            }
        }
        
    }
}

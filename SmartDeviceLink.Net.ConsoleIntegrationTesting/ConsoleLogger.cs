using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SmartDeviceLink.Net.Logging;

namespace SmartDeviceLink.Net.ConsoleIntegrationTesting
{
    public class ConsoleLogger : ILogger
    {
        public void LogVerbose(string message, object obj = null)
        {
            Console.WriteLine(message);
            if (obj != null)
                Console.WriteLine(JsonConvert.SerializeObject(obj));
        }

        public void LogError(string message, object obj = null, Exception e = null)
        {
            Console.WriteLine(message);
            if (obj != null)
                Console.WriteLine(JsonConvert.SerializeObject(obj));
            if (e != null)
                Console.WriteLine(e);
        }
    }
}

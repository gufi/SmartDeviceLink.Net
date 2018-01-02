using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Newtonsoft.Json;

namespace SmartDeviceLink.Net.Logging
{
    public class Logger : ILogger
    {
        static Logger()
        {
            SdlLogger = new Logger();
        }
        public static ILogger SdlLogger { get; set; } 
        public void LogVerbose(string message, object obj = null)
        {
            Debug.WriteLine(message);
            if(obj != null)
                Debug.WriteLine( JsonConvert.SerializeObject(obj));
        }

        public void LogError(string message, object obj = null, Exception e = null)
        {
            Debug.WriteLine(message);
            if (obj != null)
                Debug.WriteLine(JsonConvert.SerializeObject(obj));
            if (e != null)
                Debug.WriteLine(e);
        }
    }
}

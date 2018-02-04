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
        public LogLevel LogLevel { get; set; } = LogLevel.Info;

        JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented
        };
        public void LogVerbose(string message, object obj = null)
        {
            if (LogLevel < LogLevel.Verbose) return;
            Debug.WriteLine($"VERBOSE: {message}");
            if (obj != null)
                Debug.WriteLine(JsonConvert.SerializeObject(obj, settings));
        }

        public void LogDebug(string message, object obj = null)
        {
            if (LogLevel < LogLevel.Debug) return;
            Debug.WriteLine($"DEBUG: {message}");
            if (obj != null)
                Debug.WriteLine(JsonConvert.SerializeObject(obj, settings));
        }

        public void LogInfo(string message)
        {
            if (LogLevel < LogLevel.Info) return;
            Debug.WriteLine($"INFORMATION: {message}");
        }

        public void LogWarning(string message)
        {
            if (LogLevel < LogLevel.Warning) return;
            Debug.WriteLine($"WARNING: {message}");
        }


        public void LogFatal(string message, object obj = null, Exception e = null)
        {
            if (LogLevel < LogLevel.Fatal) return;
            Debug.WriteLine($"FATAL: {message}");
            if (obj != null)
                Debug.WriteLine(JsonConvert.SerializeObject(obj, settings));
            if (e != null)
                Debug.WriteLine("\t" + e);
        }

        public void LogError(string message, object obj = null, Exception e = null)
        {
            if (LogLevel < LogLevel.Error) return;
            Debug.WriteLine($"ERROR: {message}");
            if (obj != null)
                Debug.WriteLine(JsonConvert.SerializeObject(obj, settings));
            if (e != null)
                Debug.WriteLine("\t" + e);
        }
    }
}

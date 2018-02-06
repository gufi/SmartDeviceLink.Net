using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SmartDeviceLink.Net.Logging;
using SmartDeviceLink.Net.Protocol.Enums;
using SmartDeviceLink.Net.Rpc.Base;
using SmartDeviceLink.Net.Transport.Enums;

namespace SmartDeviceLink.Net.ConsoleIntegrationTesting
{
    public class ConsoleLogger : ILogger
    {

        public LogLevel LogLevel { get; set; } = LogLevel.Info;

        JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            Converters = new List<JsonConverter>()
            {
                new ByteConverter(),
                new FrameInfoConverter(),
                new ServiceTypeConverter(),
                new FrameTypeConverter(),
                new FunctionIDConverter()
            },
            Formatting = Formatting.Indented
        };
        public void LogVerbose(string message, object obj = null)
        {
            if (LogLevel < LogLevel.Verbose) return;
            Console.WriteLine($"VERBOSE: {message}");
            if (obj != null)
                Console.WriteLine( JsonConvert.SerializeObject(obj,settings));
        }

        public void LogDebug(string message, object obj = null)
        {
            if (LogLevel < LogLevel.Debug) return;
            Console.WriteLine($"DEBUG: {message}");
            if (obj != null)
                Console.WriteLine(JsonConvert.SerializeObject(obj, settings));
        }

        public void LogInfo(string message, object obj = null)
        {
            if (LogLevel < LogLevel.Info) return;
            Console.WriteLine($"INFORMATION: {message}");
            if (obj != null)
                Console.WriteLine(JsonConvert.SerializeObject(obj, settings));
        }

        public void LogWarning(string message, object obj = null)
        {
            if (LogLevel < LogLevel.Warning) return;
            Console.WriteLine($"WARNING: {message}");
            if (obj != null)
                Console.WriteLine(JsonConvert.SerializeObject(obj, settings));
        }


        public void LogFatal(string message, object obj = null, Exception e = null)
        {
            if (LogLevel < LogLevel.Fatal) return;
            Console.WriteLine($"FATAL: {message}");
            if (obj != null)
                Console.WriteLine(JsonConvert.SerializeObject(obj, settings));
            if (e != null)
                Console.WriteLine("\t" + e);
        }

        public void LogError(string message, object obj = null, Exception e = null)
        {
            if (LogLevel < LogLevel.Error) return;
            Console.WriteLine($"ERROR: {message}");
            if (obj != null)
                Console.WriteLine(JsonConvert.SerializeObject(obj, settings));
            if (e != null)
                Console.WriteLine("\t"+e);
        }
    }

    public class ByteConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var stri = Encoding.ASCII.GetString((byte[])value);
            writer.WriteValue(stri);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return existingValue;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(byte[]);
        }
    }

    public class FrameInfoConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return existingValue;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(FrameInfo);
        }
    }

    public class ServiceTypeConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return existingValue;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ServiceType);
        }
    }

    public class FrameTypeConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return existingValue;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(FrameType);
        }
    }

    public class FunctionIDConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return existingValue;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(FunctionID);
        }
    }
}

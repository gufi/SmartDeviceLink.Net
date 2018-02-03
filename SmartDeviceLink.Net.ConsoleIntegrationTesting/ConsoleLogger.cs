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
        JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            Converters = new List<JsonConverter>() {  new ByteConverter()},
            Formatting = Formatting.Indented
        };
        public void LogVerbose(string message, object obj = null)
        {
            Console.WriteLine(message);
            if (obj != null)
                Console.WriteLine("\t" + JsonConvert.SerializeObject(obj,settings));
        }

        public void LogError(string message, object obj = null, Exception e = null)
        {
            Console.WriteLine(message);
            if (obj != null)
                Console.WriteLine("\t" + JsonConvert.SerializeObject(obj, settings));
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
}

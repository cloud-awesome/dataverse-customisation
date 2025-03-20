using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace CloudAwesome.Dataverse.Core
{
    public static class SerialisationWrapper
    {
        public static T DeserialiseJsonFromFile<T>(string filePath)
        {
            var options = new JsonSerializerOptions
            {
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
            
            using (var fs = File.OpenRead(filePath))
            {
                return JsonSerializer.Deserialize<T>(fs, options);
            }
        }

        public static void SerialiseJsonToFile<T>(string filepath, T data)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };

            using var fs = File.Create(filepath);
            JsonSerializer.Serialize(fs, data, options);
        }
    }
}

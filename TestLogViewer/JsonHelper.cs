using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TestLogViewer
{
    class JsonHelper
    {
        public static JToken GetJsonFile(String fileName)
        {
            using (StreamReader file = File.OpenText(fileName))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    return JToken.ReadFrom(reader);
                }
            }
        }

        public static Task<JToken> GetJsonFileTask(String fileName)
        {
            StreamReader file = File.OpenText(fileName);
            JsonTextReader reader = new JsonTextReader(file);
            return JToken.ReadFromAsync(reader);
        }

        public static void SaveJsonFile(String fileName, JObject obj)
        {
            using (StreamWriter file = File.CreateText(fileName))
            using (JsonTextWriter writer = new JsonTextWriter(file))
            {
                obj.WriteTo(writer);
            }
        }
    }
}

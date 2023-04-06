using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Text.Json;


namespace cli_life
{
    static class JsonManagement
    {
        public static void SaveJson<T>(string filePath, T obj)
        {
            File.WriteAllText(filePath, string.Empty);

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json = System.Text.Json.JsonSerializer.Serialize(obj, options);
            File.WriteAllText(filePath, json);
        }

        public static T ReadJson<T>(string filePath)
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(File.ReadAllText(filePath));
        }
    }
}

using System.IO;
using System.Text.Json;

namespace cli_life
{
    public static class JsonManagement
    {
        public static void SaveJson<T>(string filePath, T obj)
        {
            File.WriteAllText(filePath, string.Empty);

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(obj, options);
            File.WriteAllText(filePath, json);
        }

        public static T ReadJson<T>(string filePath)
        {
            return JsonSerializer.Deserialize<T>(File.ReadAllText(filePath));
        }
    }
}


using System;
using System.IO;
using System.Text.Json;

namespace inventory.Services
{ 
    public class FileHandler
    {
        private readonly string _fullFilePath;

        public FileHandler(string filePath, string fileName)
        {
            _fullFilePath = Path.Combine(filePath, fileName);
        }

        public void EnsureFileExists()
        {
            string? directory = Path.GetDirectoryName(_fullFilePath);

            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(_fullFilePath))
            {
                File.WriteAllText(_fullFilePath, "[]");
            }
        }

        public string ReadAllText()
        {
            EnsureFileExists();

            return File.ReadAllText(_fullFilePath);
        }

        public void WriteAllText(string content)
        {
            EnsureFileExists();

            File.WriteAllText(_fullFilePath, content);
        }

        public List<T> ReadJson<T>()
        {
            string json = ReadAllText();

            return JsonSerializer.Deserialize<List<T>>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<T>();
        }

        public void WriteJson<T>(List<T> data)
        {
            string json = JsonSerializer.Serialize(
                data,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            WriteAllText(json);
        }
    }
}

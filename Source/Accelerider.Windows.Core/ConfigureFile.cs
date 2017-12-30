using System.IO;
using System.Linq;
using Accelerider.Windows.Infrastructure.Interfaces;
using Newtonsoft.Json.Linq;

namespace Accelerider.Windows.Core
{
    public class ConfigureFile : IConfigureFile
    {
        private JObject _storage;
        private string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "config.json");

        public ConfigureFile() => Load();

        public string this[string key]
        {
            get => _storage.Value<string>(key);
            set => _storage[key] = value;
        }

        public bool Contains(string key) => _storage.Values().Any(token => token.Path == key);

        public T GetValue<T>(string key) => _storage.Value<T>(key);

        public void Delete()
        {
            File.WriteAllText(_filePath, string.Empty);
            File.Delete(_filePath);
        }

        public void Load(string filePath = null)
        {
            if (!string.IsNullOrEmpty(filePath)) _filePath = filePath;

            if (!File.Exists(_filePath))
            {
                _storage = new JObject();
                Save();
            }
            _storage = JObject.Parse(File.ReadAllText(_filePath));
        }

        public void Save() => File.WriteAllText(_filePath, _storage.ToString());
    }
}

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Accelerider.Windows.Infrastructure.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Accelerider.Windows.Core
{
    public class ConfigureFile : IConfigureFile
    {
        private JObject _storage;
        private string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "Accelerider.info");

        public object this[string key]
        {
            get => JsonConvert.DeserializeObject(_storage[key]?.ToString() ?? string.Empty);
            set
            {
                if (EqualityComparer<object>.Default.Equals(_storage[key], value)) return;
                _storage[key] = JsonConvert.SerializeObject(value, Formatting.Indented);
                Save();
            }
        }

        public bool Contains(string key) => _storage.Values().Any(token => token.Path == key);

        public T GetValue<T>(string key) => JsonConvert.DeserializeObject<T>(_storage[key]?.ToString() ?? string.Empty);

        public IConfigureFile Load(string filePath = null)
        {
            if (!string.IsNullOrEmpty(filePath)) _filePath = filePath;

            if (!File.Exists(_filePath))
            {
                _storage = new JObject(JObject.Parse("{}"));
                Save();
            }
            _storage = JObject.Parse(File.ReadAllText(_filePath));

            return this;
        }

        public IConfigureFile Clear()
        {
            _storage = new JObject();
            Save();
            return this;
        }

        public void Delete()
        {
            Clear();
            File.Delete(_filePath);
        }

        private void Save() => WriteToLocal(_filePath, _storage.ToString(Formatting.Indented));

        private void WriteToLocal(string path, string text)
        {
            try
            {
                File.WriteAllText(path, text);
            }
            catch (IOException)
            {
                WriteToLocal(path, text);
            }
        }
    }
}

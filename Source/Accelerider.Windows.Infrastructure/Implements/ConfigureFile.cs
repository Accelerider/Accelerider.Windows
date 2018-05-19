using System.Collections.Generic;
using System.IO;
using System.Linq;
using Accelerider.Windows.Infrastructure.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Accelerider.Windows.Infrastructure
{
    public class ConfigureFile : IConfigureFile
    {
        private JObject _storage;
        private string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "Accelerider.conf");

        public event ValueChangedEventHandler ValueChanged;

        public bool Contains(string key) => _storage.Values().Any(token => token.Path == key);

        public T GetValue<T>(string key) => JsonConvert.DeserializeObject<T>(_storage[key]?.ToString() ?? string.Empty);

        public IConfigureFile SetValue<T>(string key, T value)
        {
            if (EqualityComparer<object>.Default.Equals(_storage[key], value)) return this;

            _storage[key] = JsonConvert.SerializeObject(value, Formatting.Indented);
            Save();
            ValueChanged?.Invoke(this, new ValueChangedEventArgs(key));

            return this;
        }

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

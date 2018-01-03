using System.IO;
using System.Linq;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Accelerider.Windows.Core
{
    public class ConfigureFile : IConfigureFile
    {
        private JObject _storage;
        private string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "Accelerider.info");

        public ConfigureFile() => Load();

        public object this[string key]
        {
            get => JsonConvert.DeserializeObject(_storage[key].ToString());
            set => _storage[key] = JsonConvert.SerializeObject(value);
        }

        public bool Contains(string key) => _storage.Values().Any(token => token.Path == key);

        public T GetValue<T>(string key) => JsonConvert.DeserializeObject<T>(_storage[key].ToString());

        public void Delete()
        {
            _storage = new JObject();
            Save();
            File.Delete(_filePath);
        }

        public void Load(string filePath = null)
        {
            if (!string.IsNullOrEmpty(filePath)) _filePath = filePath;

            if (!File.Exists(_filePath))
            {
                _storage = new JObject(JObject.Parse("{}"));
                Save();
            }
            _storage = JObject.Parse(File.ReadAllText(_filePath).DecryptByRijndael());
        }

        public void Save()
        {
            WriteToLocal(_filePath, _storage.ToString().EncryptByRijndael());
        }

        private void WriteToLocal(string path, string text)
        {
            try
            {
                File.WriteAllText(path, text);
            }
            catch (IOException e)
            {
                WriteToLocal(path, text);
            }
        }
    }
}

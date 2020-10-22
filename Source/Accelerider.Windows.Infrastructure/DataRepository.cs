using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Accelerider.Windows.Infrastructure
{
    public interface IDataRepository
    {
        T Get<T>(string path = null, bool isGlobal = false) where T : INotifyPropertyChanged;

        void Save(object data);
    }

    public class DataRepository : IDataRepository
    {
        private const string EmptyJson = "{ }";

        private readonly ConcurrentDictionary<string, (object Settings, bool IsGlobal)> _cache =
            new ConcurrentDictionary<string, (object Settings, bool IsGlobal)>();

        private Func<string> _keyGetter;
        private string _key;

        private string Username
        {
            get
            {
                if (string.IsNullOrEmpty(_key))
                {
                    _key = _keyGetter?.Invoke();
                    if (!string.IsNullOrEmpty(_key))
                    {
                        _keyGetter = null;
                    }
                }

                return _key;
            }
        }

        public DataRepository(Func<string> keyGetter)
        {
            _keyGetter = keyGetter;
        }

        public T Get<T>(string path = null, bool isGlobal = false) where T : INotifyPropertyChanged
        {
            path = path ?? GetSettingsFilePath(typeof(T), isGlobal);

            if (!_cache.TryGetValue(path, out var settingsCache))
            {
                var newSettingsCache = NewSettingsCache<T>(isGlobal, path);
                _cache[path] = newSettingsCache;
                settingsCache = newSettingsCache;
            }

            return (T)settingsCache.Settings;
        }

        public void Save(object data)
        {
            var path = _cache.First(item => ReferenceEquals(item.Value.Settings, data)).Key;

            WriteToLocal(path, data.ToJson(Formatting.Indented, false));
        }

        private (T Settings, bool IsGlobal) NewSettingsCache<T>(bool isGlobal, string path)
            where T : INotifyPropertyChanged
        {
            if (!File.Exists(path))
            {
                WriteToLocal(path, EmptyJson);
            }

            var instance = File.ReadAllText(path).ToObject<T>();
            instance.PropertyChanged += SettingsOnPropertyChanged;
            return (instance, isGlobal);
        }

        private void SettingsOnPropertyChanged(object sender, PropertyChangedEventArgs e) => Save(sender);

        private string GetSettingsFilePath(Type type, bool isGlobal = false)
        {
            var name = type.Name.UpperCamelCaseToDelimiterSeparated();

            var key = isGlobal ? string.Empty : $".{Username}";
            return Path.Combine(AcceleriderFolders.Database, $"{name}{key}.json");
        }

        private static void WriteToLocal(string path, string contents)
        {
            try
            {
                File.WriteAllText(path, contents);
            }
            catch (IOException)
            {
                WriteToLocal(path, contents);
            }
        }
    }
}

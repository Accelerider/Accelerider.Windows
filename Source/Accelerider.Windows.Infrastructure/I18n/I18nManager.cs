using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Threading;
using System.Windows;

namespace Accelerider.Windows.Infrastructure.I18n
{
    public class I18nManager
    {
        private readonly ConcurrentDictionary<string, ResourceManager> _resourceManagerStorage = new ConcurrentDictionary<string, ResourceManager>();

        public event Action CurrentUICultureChanged;

        public static I18nManager Instance { get; } = new I18nManager();

        private I18nManager() { }

        public CultureInfo CurrentUICulture
        {
            get => Thread.CurrentThread.CurrentUICulture;
            set
            {
                if (EqualityComparer<CultureInfo>.Default.Equals(value, Thread.CurrentThread.CurrentUICulture)) return;

                Thread.CurrentThread.CurrentUICulture = value;
                Thread.CurrentThread.CurrentCulture = value;
                OnCurrentUICultureChanged();
            }
        }

        public IEnumerable<CultureInfo> AvailableCultureInfos => new[]
        {
            new CultureInfo("zh-CN"),
            new CultureInfo("en-US")
        };

        public void AddResourceManager(ResourceManager resourceManager)
        {
            if (_resourceManagerStorage.ContainsKey(resourceManager.BaseName))
                throw new ArgumentException("", nameof(resourceManager));

            _resourceManagerStorage[resourceManager.BaseName] = resourceManager;
        }

        private void OnCurrentUICultureChanged() => CurrentUICultureChanged?.Invoke();

        public object Get(ComponentResourceKey key)
        {
            return GetCurrentResourceManager(key.TypeInTargetAssembly.FullName)?.GetString(key.ResourceId.ToString(), CurrentUICulture)
                   ?? $"<MISSING: {key}>";
        }

        private ResourceManager GetCurrentResourceManager(string key)
        {
            return _resourceManagerStorage.TryGetValue(key, out var value) ? value : null;
        }
    }
}

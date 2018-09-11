using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows;

namespace Accelerider.Windows.Infrastructure.I18n
{
    public class LanguageManager
    {
        private readonly ConcurrentDictionary<string, ResourceManager> _resourceManagerStorage = new ConcurrentDictionary<string, ResourceManager>();

        public event Action CurrentUICultureChanged;

        public static LanguageManager Instance { get; } = new LanguageManager();

        private LanguageManager() { }

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

        public void AddResoureManager(ResourceManager resourceManager)
        {
            if (_resourceManagerStorage.ContainsKey(Assembly.GetCallingAssembly().FullName))
                throw new ArgumentException("", nameof(resourceManager));

            _resourceManagerStorage[Assembly.GetCallingAssembly().FullName] = resourceManager;
        }

        private void OnCurrentUICultureChanged() => CurrentUICultureChanged?.Invoke();

        public object Translate(ComponentResourceKey key)
        {
            return GetCurrentResoureManager(key.Assembly)?.GetString(key.ResourceId.ToString(), CurrentUICulture)
                   ?? $"<MISSING: {key}>";
        }

        private ResourceManager GetCurrentResoureManager(Assembly assembly)
        {
            return _resourceManagerStorage.TryGetValue(assembly.FullName, out var value) ? value : null;
        }
    }
}

using System.ComponentModel;
using System.Windows;

namespace Accelerider.Windows.Infrastructure.I18n
{
    public class I18nSource : INotifyPropertyChanged
    {
        private readonly ComponentResourceKey _key;

        public event PropertyChangedEventHandler PropertyChanged;

        public I18nSource(FrameworkElement element, ComponentResourceKey key)
        {
            _key = key;
            element.Loaded += OnLoaded;
            element.Unloaded += OnUnloaded;
        }

        public object Value => LanguageManager.Instance.Translate(_key);

        private void OnCurrentUICultureChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            LanguageManager.Instance.CurrentUICultureChanged += OnCurrentUICultureChanged;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            LanguageManager.Instance.CurrentUICultureChanged -= OnCurrentUICultureChanged;
        }
    }
}

using System.ComponentModel;
using System.Windows;

namespace Accelerider.Windows.Infrastructure.I18n
{
    public class I18nSource : INotifyPropertyChanged
    {
        private readonly ComponentResourceKey _key;

        public event PropertyChangedEventHandler PropertyChanged;

        public I18nSource(ComponentResourceKey key, FrameworkElement element = null)
        {
            _key = key;

            if (element != null)
            {
                element.Loaded += OnLoaded;
                element.Unloaded += OnUnloaded;
            }
        }

        public object Value => I18nManager.Instance.Get(_key);

        private void OnCurrentUICultureChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            I18nManager.Instance.CurrentUICultureChanged += OnCurrentUICultureChanged;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            I18nManager.Instance.CurrentUICultureChanged -= OnCurrentUICultureChanged;
        }

        public static implicit operator I18nSource(ComponentResourceKey resourceKey) => new I18nSource(resourceKey);
    }
}

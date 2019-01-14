using System.ComponentModel;
using System.Windows.Data;
using Prism.Mvvm;

namespace System.Windows.Extensions.PropertyHelpers
{
    public class PropertyProxy<T> : BindableBase
    {
        public static PropertyProxy<T> Create(INotifyPropertyChanged owner, string propertyName)
        {
            return new PropertyProxy<T>(owner, propertyName);
        }

        public static PropertyProxy<T> CreateFromBinding(Binding binding, FrameworkElement frameworkElement = null)
        {
            if (binding.Source != null)
            {
                return new PropertyProxy<T>((INotifyPropertyChanged)binding.Source, binding.Path.Path);
            }

            if (frameworkElement != null)
            {
                return new PropertyProxy<T>(frameworkElement, binding.Path.Path);
            }

            throw new ArgumentNullException();
        }

        private readonly FrameworkElement _frameworkElement;
        private readonly string _expression;

        private PropertyObserver _propertyObserver;

        public T Value
        {
            get
            {
                if (_propertyObserver == null)
                {
                    if (_frameworkElement.IsLoaded)
                    {
                        Initialize((INotifyPropertyChanged)_frameworkElement.DataContext, _expression);
                    }
                    else
                    {
                        _frameworkElement.Loaded += FrameworkElementOnLoaded;
                    }
                }

                return _propertyObserver != null ? (T)_propertyObserver.PropertyValue : default;
            }
        }

        private void FrameworkElementOnLoaded(object sender, RoutedEventArgs e)
        {
            var frameworkElement = (FrameworkElement)sender;
            frameworkElement.Loaded -= FrameworkElementOnLoaded;

            Initialize((INotifyPropertyChanged)frameworkElement.DataContext, _expression);
            RaisePropertyChanged(nameof(Value));
        }

        private PropertyProxy(INotifyPropertyChanged owner, string expression)
        {
            Initialize(owner, expression);
        }

        private PropertyProxy(FrameworkElement frameworkElement, string propertyName)
        {
            _frameworkElement = frameworkElement;
            _expression = propertyName;
        }

        private void Initialize(INotifyPropertyChanged owner, string expression)
        {
            if (_propertyObserver != null) return;

            _propertyObserver = PropertyObserver.Observers(
                owner, expression, () => RaisePropertyChanged(nameof(Value)));
        }
    }
}

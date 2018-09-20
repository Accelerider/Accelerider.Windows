using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Accelerider.Windows.Infrastructure.I18n
{
    [MarkupExtensionReturnType(typeof(object))]
    public class I18nExtensions : MarkupExtension
    {
        [ConstructorArgument(nameof(Key))]
        public ComponentResourceKey Key { get; set; }

        public I18nExtensions() { }

        public I18nExtensions(ComponentResourceKey key) => Key = key;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            //if (Binding != null) return ProvideValueFromBinding(serviceProvider, Binding);
            if (Key == null)
                throw new NullReferenceException($"{nameof(Key)} cannot be null at the same time.");

            return ProvideValueFromKey(serviceProvider, Key);
        }

        private object ProvideValueFromKey(IServiceProvider serviceProvider, ComponentResourceKey key)
        {
            if (!(serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget provideValueTarget))
                throw new ArgumentException($"The {nameof(serviceProvider)} must implement {nameof(IProvideValueTarget)} interface.");

            if (provideValueTarget.TargetObject.GetType().FullName == "System.Windows.SharedDp") return this;

            if (!(provideValueTarget.TargetObject is FrameworkElement frameworkElement))
                throw new ArgumentException($"The {nameof(frameworkElement)} must be a derived class from {nameof(FrameworkElement)}.");

            return new Binding(nameof(I18nSource.Value))
            {
                Source = new I18nSource(key, frameworkElement),
                Mode = BindingMode.OneWay
            }.ProvideValue(serviceProvider);
        }


        //private BindableBase _dataContext;

        //[ConstructorArgument(nameof(Binding))]
        //public Binding Binding { get; }

        //public UiStringExtension(Binding binding) => Binding = binding;

        //private object ProvideValueFromBinding(IServiceProvider serviceProvider, Binding binding)
        //{
        //    if (!(serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget provideValueTarget))
        //        throw new ArgumentException($"The {nameof(serviceProvider)} must implement {nameof(IProvideValueTarget)} interface.");

        //    if (provideValueTarget.TargetObject.GetType().FullName == "System.Windows.SharedDp") return this;

        //    if (!(provideValueTarget.TargetObject is FrameworkElement frameworkElement))
        //        throw new ArgumentException($"The {nameof(frameworkElement)} must be a derived class from {nameof(FrameworkElement)}.");

        //    var bindingCode = binding.GetHashCode();
        //    var frameworkCode = frameworkElement.GetHashCode();
        //    var dataCode = frameworkElement.DataContext.GetHashCode();

        //    if (frameworkElement.DataContext is BindableBase dataContext && binding.Source == null)
        //    {
        //        //_dataContext = dataContext;
        //        //frameworkElement.Loaded += OnLoaded;
        //        //frameworkElement.Unloaded += OnUnloaded;
        //        LanguageManager.Instance.WeakEvent += (sender, e) => dataContext.RaisePropertyChanged(binding.Path.Path);
        //        binding.Source = dataContext;
        //    }

        //    return binding.ProvideValue(serviceProvider);
        //}

        //private void OnLoaded(object sender, RoutedEventArgs e) => LanguageManager.Instance.WeakEvent += OnCultureInfoChanged;

        //private void OnUnloaded(object sender, RoutedEventArgs e) => LanguageManager.Instance.WeakEvent -= OnCultureInfoChanged;

        //private void OnCultureInfoChanged(object sender, EventArgs e) => _dataContext.RaisePropertyChanged(Binding.Path.Path);
    }
}

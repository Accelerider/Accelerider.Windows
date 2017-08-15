using System;
using System.Windows;
using System.Globalization;
using System.Reflection;

namespace Accelerider.Windows.ViewModels
{
    public static class ViewModelLocator
    {
        #region The AutoWireViewModel attached property.
        public static DependencyProperty AutoWireViewModelProperty = DependencyProperty.RegisterAttached("AutoWireViewModel", typeof(bool), typeof(ViewModelLocator), new PropertyMetadata(false, AutoWireViewModelChanged));
        public static bool GetAutoWireViewModel(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoWireViewModelProperty);
        }
        public static void SetAutoWireViewModel(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoWireViewModelProperty, value);
        }
        #endregion

        private static readonly Func<Type, Type> ViewModelTypeResolver = viewType =>
        {
            var viewName = viewType.FullName.Replace(".Views.", ".ViewModels.");
            var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
            var suffix = viewName.EndsWith("View") ? "Model" : "ViewModel";
            var viewModelName = string.Format(CultureInfo.InvariantCulture, "{0}{1}, {2}", viewName, suffix, viewAssemblyName);
            return Type.GetType(viewModelName);
        };

        public static Func<Type, object> ViewModelFactory { get; set; } = type => Activator.CreateInstance(type);


        private static void AutoWireViewModelChanged(DependencyObject view, DependencyPropertyChangedEventArgs eventArgs)
        {
            if (!(bool)eventArgs.NewValue) return;
            var viewType = view.GetType();
            if (view is FrameworkElement element)
            {
                element.DataContext = ViewModelFactory(ViewModelTypeResolver(viewType));
                if (element.DataContext is ViewModelBase viewModel)
                {
                    element.Loaded += async (sender, e) => await viewModel.OnViewLoadedAsync();
                }
            }
        }
    }
}

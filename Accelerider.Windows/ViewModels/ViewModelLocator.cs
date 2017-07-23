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

        #region The AutoRegisterLoadedHandler attached property
        public static readonly DependencyProperty AutoRegisterLoadedHandlerProperty = DependencyProperty.RegisterAttached("AutoRegisterLoadedHandler", typeof(bool), typeof(ViewModelLocator), new PropertyMetadata(false, AutoRegisterLoadedHandlerChanged));
        public static void SetAutoRegisterLoadedHandler(DependencyObject element, bool value)
        {
            element.SetValue(AutoRegisterLoadedHandlerProperty, value);
        }
        public static bool GetAutoRegisterLoadedHandler(DependencyObject element)
        {
            return (bool)element.GetValue(AutoRegisterLoadedHandlerProperty);
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


        private static void AutoWireViewModelChanged(DependencyObject view, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)e.NewValue) return;
            var viewType = view.GetType();
            if (view is FrameworkElement element)
                element.DataContext = ViewModelFactory(ViewModelTypeResolver(viewType));
        }

        private static void AutoRegisterLoadedHandlerChanged(DependencyObject d, DependencyPropertyChangedEventArgs eventArgs)
        {
            if (!(bool)eventArgs.NewValue) return;
            var view = d as FrameworkElement;
            var viewModel = view?.DataContext as ViewModelBase ?? throw new NullReferenceException();
            view.Loaded += async (sender, e) => await viewModel.OnViewLoaded();
        }
    }
}

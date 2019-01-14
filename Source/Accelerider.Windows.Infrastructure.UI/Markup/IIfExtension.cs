using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Extensions.PropertyHelpers;
using System.Windows.Markup;
using Prism.Mvvm;

namespace System.Windows.Extensions.Markup
{
    // ReSharper disable once InconsistentNaming
    [MarkupExtensionReturnType(typeof(object))]
    public class IIfExtension : MarkupExtension
    {
        [ConstructorArgument(nameof(Condition))]
        public Binding Condition { get; set; }

        [ConstructorArgument(nameof(TrueValue))]
        public object TrueValue { get; set; }

        [ConstructorArgument(nameof(FalseValue))]
        public object FalseValue { get; set; }

        public IIfExtension() { }

        public IIfExtension(Binding condition, object trueValue, object falseValue)
        {
            Condition = condition;
            TrueValue = trueValue;
            FalseValue = falseValue;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (!(serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget provideValueTarget))
                throw new InvalidOperationException($"The {nameof(serviceProvider)} must implement {nameof(IProvideValueTarget)} interface.");

            if (provideValueTarget.TargetObject.GetType().FullName == "System.Windows.SharedDp") return this;

            var dependencyObject = provideValueTarget.TargetObject as DependencyObject;

            var a = provideValueTarget.TargetProperty;

            var frameworkElement = dependencyObject?.TryFindParent<FrameworkElement>();

            return new Binding(nameof(ConditionExpressionObject.Value))
            {
                Source = new ConditionExpressionObject(Condition, TrueValue, FalseValue, frameworkElement),
                Mode = BindingMode.OneWay
            }.ProvideValue(serviceProvider);
        }

        private class ConditionExpressionObject : BindableBase
        {
            private readonly PropertyProxy<bool> _condition;
            private readonly object _trueValue;
            private readonly object _falseValue;

            private readonly PropertyProxy<object> _trueValuePropertyProxy;
            private readonly PropertyProxy<object> _falseValuePropertyProxy;

            public object Value => _condition.Value
                ? _trueValuePropertyProxy?.Value ?? _trueValue
                : _falseValuePropertyProxy?.Value ?? _falseValue;

            public ConditionExpressionObject(Binding condition, object trueValue, object falseValue, FrameworkElement frameworkElement)
            {
                TrySetPropertyProxy(condition, frameworkElement, ref _condition);

                if (!TrySetPropertyProxy(trueValue, frameworkElement, ref _trueValuePropertyProxy))
                    _trueValue = trueValue;

                if (!TrySetPropertyProxy(falseValue, frameworkElement, ref _falseValuePropertyProxy))
                    _falseValue = falseValue;
            }

            private bool TrySetPropertyProxy<T>(object value, FrameworkElement frameworkElement, ref PropertyProxy<T> proxyStorage)
            {
                if (value is Binding binding)
                {
                    proxyStorage = PropertyProxy<T>.CreateFromBinding(binding, frameworkElement);
                    proxyStorage.PropertyChanged += OnPropertyChanged;
                    return true;
                }

                return false;
            }

            private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(PropertyProxy<object>.Value)) RaisePropertyChanged(nameof(Value));
            }
        }
    }
}

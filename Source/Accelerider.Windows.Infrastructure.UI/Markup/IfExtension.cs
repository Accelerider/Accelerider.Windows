using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace System.Windows.Extensions.Markup
{
    public class IfExtension : Binding
    {
        [ConstructorArgument(nameof(TrueValue))]
        public object TrueValue { get; set; }

        [ConstructorArgument(nameof(FalseValue))]
        public object FalseValue { get; set; }

        public IfExtension() => SetConverter();

        public IfExtension(string path) : base(path)
        {
            SetConverter();
        }

        private void SetConverter()
        {
            Converter = new ConverterImpl(() => TrueValue, () => FalseValue);
        }

        private class ConverterImpl : IValueConverter
        {
            private readonly Func<object> _trueValue;
            private readonly Func<object> _falseValue;

            public ConverterImpl(Func<object> trueValue, Func<object> falseValue)
            {
                _trueValue = trueValue;
                _falseValue = falseValue;
            }

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return value.CastTo<bool>() ? _trueValue() : _falseValue();
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return ReferenceEquals(value, _trueValue());
            }
        }
    }
}

namespace Accelerider.Windows.Resources.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class LessThan1000Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var number = (int)value;
            if (number <= 0) return null;
            if (number < 1000) return number;
            return "999+";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

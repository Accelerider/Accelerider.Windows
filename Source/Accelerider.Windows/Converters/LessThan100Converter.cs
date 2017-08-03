using System;
using System.Globalization;
using System.Windows.Data;

namespace Accelerider.Windows.Converters
{
    public class LessThan100Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var number = (int) value;
            return number < 100 ? number.ToString() : "99+";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Globalization;
using System.Windows.Data;

namespace Accelerider.Windows.Infrastructure.Converters
{
    public class MinusConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var left = double.Parse(values[0].ToString());
            var right = double.Parse(values[1].ToString());
            return left - right;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

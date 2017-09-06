using System;
using System.Globalization;
using System.Windows.Data;

namespace Accelerider.Windows.Converters
{
    public class PlusConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var left = double.Parse(values[0].ToString());
            var right = double.Parse(values[1].ToString());
            double? result = left + right;
            return result <= 0 ? null : result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

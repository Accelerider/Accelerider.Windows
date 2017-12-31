namespace Accelerider.Windows.Resources.Converters
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;

    public class PlusConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double? result = values.Select(value => double.Parse(value.ToString())).Aggregate((sum, value) => sum + value);
            return result <= 0 ? null : result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

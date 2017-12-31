namespace Accelerider.Windows.Resources.Converters
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;

    public class StringToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var firstLetter = value.ToString().First().ToString();
            var temp = new DrawingVisual();
            var c = temp.RenderOpen();

            var typeface = new Typeface(new FontFamily("Microsoft YaHei"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            c.DrawRectangle(Brushes.BlueViolet, null, new Rect(new Size(32, 32)));
            c.DrawText(new FormattedText(firstLetter, culture, FlowDirection.LeftToRight, typeface, 16, Brushes.White), new Point(8, 5));
            c.Close();
            var image = new DrawingImage(temp.Drawing);
            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

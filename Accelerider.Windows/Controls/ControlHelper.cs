using System.Windows;
using System.Windows.Media;

namespace Accelerider.Windows.Controls
{
    public class ControlHelper
    {
        public static readonly DependencyProperty MouseOverBackgoundProperty = DependencyProperty.RegisterAttached("MouseOverBackgound", typeof(Brush), typeof(ControlHelper), new PropertyMetadata(null));
        public static Brush GetMouseOverBackgound(DependencyObject obj)
        {
            return (Brush)obj.GetValue(MouseOverBackgoundProperty);
        }
        public static void SetMouseOverBackgound(DependencyObject obj, Brush value)
        {
            obj.SetValue(MouseOverBackgoundProperty, value);
        }
    }
}

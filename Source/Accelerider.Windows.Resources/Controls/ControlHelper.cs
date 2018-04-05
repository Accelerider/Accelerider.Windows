namespace Accelerider.Windows.Resources.Controls
{
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;

    public class ControlHelper
    {


        public static readonly DependencyProperty MouseOverBackgroundProperty = DependencyProperty.RegisterAttached("MouseOverBackground", typeof(Brush), typeof(ControlHelper), new PropertyMetadata(null));
        public static Brush GetMouseOverBackground(DependencyObject obj)
        {
            return (Brush)obj.GetValue(MouseOverBackgroundProperty);
        }
        public static void SetMouseOverBackground(DependencyObject obj, Brush value)
        {
            obj.SetValue(MouseOverBackgroundProperty, value);
        }

        public static readonly DependencyProperty MouseOverForegroundProperty = DependencyProperty.RegisterAttached("MouseOverForeground", typeof(Brush), typeof(ControlHelper), new PropertyMetadata(null));
        public static Brush GetMouseOverForeground(DependencyObject obj)
        {
            return (Brush)obj.GetValue(MouseOverForegroundProperty);
        }
        public static void SetMouseOverForeground(DependencyObject obj, Brush value)
        {
            obj.SetValue(MouseOverForegroundProperty, value);
        }



        public static readonly DependencyProperty FollowTargetProperty = DependencyProperty.RegisterAttached("FollowTarget", typeof(DependencyObject), typeof(ControlHelper), new PropertyMetadata(null, FollowTargetChanged));
        public static DependencyObject GetFollowTarget(DependencyObject obj)
        {
            return (DependencyObject)obj.GetValue(FollowTargetProperty);
        }
        public static void SetFollowTarget(DependencyObject obj, DependencyObject value)
        {
            obj.SetValue(FollowTargetProperty, value);
        }

        private static void FollowTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is DependencyObject target &&
                Window.GetWindow(target) is Window window &&
                d is Popup popup)
            {
                window.LocationChanged += (sender, eventArgs) =>
                {
                    var backup = popup.HorizontalOffset;
                    popup.HorizontalOffset++;
                    popup.HorizontalOffset = backup;
                };
            }
        }


    }
}

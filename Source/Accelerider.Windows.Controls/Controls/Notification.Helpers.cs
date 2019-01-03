using System.Windows;

namespace Accelerider.Windows.Controls
{
    public partial class Notification
    {
        public static readonly DependencyProperty AsNotificationHostProperty = DependencyProperty.RegisterAttached(
            "AsNotificationHost", typeof(bool), typeof(Notification), new PropertyMetadata(false, AsNotificationHostChanged));

        public static void SetAsNotificationHost(DependencyObject element, bool value)
        {
            element.SetValue(AsNotificationHostProperty, value);
        }

        public static bool GetAsNotificationHost(DependencyObject element)
        {
            return (bool)element.GetValue(AsNotificationHostProperty);
        }

        private static void AsNotificationHostChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = (Window)d;


        }


        #region Helper methods

        public static void Success(NotificationConfigure configure)
        {

        }

        public static void Error(NotificationConfigure configure)
        {

        }

        public static void Info(NotificationConfigure configure)
        {

        }

        public static void Warning(NotificationConfigure configure)
        {

        }

        public static void Open(NotificationConfigure configure)
        {

        }

        public static void Close(string key)
        {

        }

        #endregion
    }
}

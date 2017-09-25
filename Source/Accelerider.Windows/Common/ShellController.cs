using System.Linq;
using System.Windows;

namespace Accelerider.Windows.Common
{
    public static class ShellController
    {
        public static void Show<T>(T shell = null) where T : Window, new()
        {
            Application.Current.MainWindow = shell ?? new T();
            Application.Current.MainWindow.Loaded += SingletonProcess.OnWindowLoaded;
            Application.Current.MainWindow.Show();
        }

        public static void Close<T>() where T : Window
        {
            var shell = Application.Current.Windows.OfType<Window>().FirstOrDefault(window => window is T);
            shell?.Close();
        }

        public static void Switch<TClose, TShow>()
            where TShow : Window, new()
            where TClose : Window
        {
            Show<TShow>();
            Close<TClose>();
        }
    }
}

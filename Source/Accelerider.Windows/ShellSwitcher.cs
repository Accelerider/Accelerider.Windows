using System.Linq;
using System.Windows;

namespace Accelerider.Windows
{
    public static class ShellSwitcher
    {
        public static void Show<T>(T window = null) where T : Window, new()
        {
            var shell = Application.Current.MainWindow = window ?? new T();
            shell.Loaded += ProcessController.OnWindowLoaded;
            shell.Show();
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

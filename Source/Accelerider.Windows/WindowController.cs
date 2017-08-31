using System.Windows;

namespace Accelerider.Windows
{
    public static class WindowController
    {
        public static void Show<T>() where T : Window, new()
        {
            Application.Current.MainWindow = new T();
            Application.Current.MainWindow.Loaded += SingletonProcess.OnWindowLoaded;
            Application.Current.MainWindow.Show();
        }

        public static void Close<T>() where T : Window
        {
            var window = FirstOrDefault<T>();
            window?.Close();
        }

        public static void Switch<TClose, TShow>() 
            where TShow : Window, new()
            where TClose : Window
        {
            Show<TShow>();
            Close<TClose>();
        }

        private static Window FirstOrDefault<T>()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is T) return window;
            }
            return null;
        }
    }
}

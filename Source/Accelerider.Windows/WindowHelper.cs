using System;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Accelerider.Windows.Properties;
using log4net;
using Prism.Regions;
using Unity;

namespace Accelerider.Windows
{
    public static class WindowHelper
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(WindowHelper));
        private static bool _creatingWindow;

        #region Switch operation

        public static IUnityContainer Container { get; set; }

        public static void SwitchTo<T>(bool keepSingleWindow = true) where T : Window, new()
        {
            try
            {
                SwitchToInternal<T>(keepSingleWindow);
            }
            catch (Exception e)
            {
                RecordWindowSwitchingException(e, typeof(T));

                throw;
            }
        }

        private static void RecordWindowSwitchingException(Exception e, Type windowType)
        {
            var windowsInfo = string.Join(", ",
                Application.Current.Windows.Cast<Window>().Select(item => item.GetType().Name));

            Logger.Error($"{e.GetRootException().Message}. {Environment.NewLine}" +
                         $"Switching to the {windowType.Name}. {Environment.NewLine}" +
                         $"CurrentWindow = {Application.Current.MainWindow?.GetType().Name}. {Environment.NewLine}" +
                         $"Windows = {windowsInfo}", e);
        }

        private static void SwitchToInternal<T>(bool keepSingleWindow) where T : Window, new()
        {
            var existedWindow = GetWindow<T>();
            if (existedWindow.ActivateWindow()) return;

            if (_creatingWindow) return;

            _creatingWindow = true;
            var previousWindows = keepSingleWindow
                ? Application.Current.Windows.OfType<Window>().ToList()
                : Application.Current.Windows.OfType<T>().Cast<Window>().ToList();

            CreateWindow<T>().Show();

            previousWindows.ForEach(window => window.Close());
            _creatingWindow = false;
        }

        private static Window CreateWindow<T>() where T : Window, new()
        {
            var newRegionManager = Container.Resolve<IRegionManager>().CreateRegionManager();
            Container.RegisterInstance(newRegionManager);

            var window = Application.Current.MainWindow = new T();
            window.Loaded += OnWindowLoaded;

            RegionManager.SetRegionManager(window, newRegionManager);
            RegionManager.UpdateRegions();

            return window;
        }

        private static void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (sender == null ||
                !(sender is Visual visual) ||
                !(PresentationSource.FromVisual(visual) is HwndSource hwndSource)) return;

            Settings.Default.WindowHandle = (long)hwndSource.Handle;
        }
        #endregion

        public static T GetWindow<T>(Func<T, bool> predicate = null) where T : Window
        {
            if (Application.Current.MainWindow is T)
                return (T)Application.Current.MainWindow;

            var windows = predicate == null
                ? Application.Current.Windows.OfType<T>().ToList()
                : Application.Current.Windows.OfType<T>().Where(predicate).ToList();

            T result = null;
            if (windows.Count == 1)
            {
                result = windows.Single();
            }
            else if (windows.Count > 1)
            {
                result = windows.Last();
                windows.Remove(result);
                windows.ForEach(item => item.Close());
            }

            return result;
        }
    }
}

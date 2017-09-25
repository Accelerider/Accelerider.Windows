using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Accelerider.Windows.Properties;

namespace Accelerider.Windows.Common
{
    public class SingletonProcess
    {
        private const int SW_SHOWNORMAL = 1;
        private const int SW_RESTORE = 9;
        private const string PROCESS_NAME = "Accelerider.Windows";


        #region Win32 API functions
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool IsZoomed(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool FlashWindow(IntPtr hWnd, bool bInvert);
        #endregion


        private static Mutex _mutex;


        public static void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            IntPtr hwnd = ((HwndSource)PresentationSource.FromVisual((Visual)sender)).Handle;
            Settings.Default.WindowHandle = (long)hwnd;
            Settings.Default.Save();
        }

        public void Check()
        {
            _mutex = new Mutex(true, PROCESS_NAME, out bool isNew);
            if (!isNew)
            {
                ActivateExistedWindow();
                Environment.Exit(0);
            }
        }

        private void ActivateExistedWindow()
        {
            IntPtr windowHandle = (IntPtr)Settings.Default.WindowHandle;

            SetForegroundWindow(windowHandle);
            ShowWindowAsync(windowHandle, IsIconic(windowHandle) ? SW_RESTORE : SW_SHOWNORMAL);
            GetForegroundWindow();
            FlashWindow(windowHandle, true);
        }
    }
}

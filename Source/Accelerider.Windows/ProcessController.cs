using System;
using System.Threading;
using System.Windows;
using Accelerider.Windows.Properties;
using System.Diagnostics;
using System.IO;
using log4net;

namespace Accelerider.Windows
{
    public static class ProcessController
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ProcessController));
        private static volatile EventWaitHandle _keepAliveEvent;

        public const string LauncherName = "Launcher.exe";
        public static readonly string LauncherPath = Path.Combine(Environment.CurrentDirectory, LauncherName);

        public static void Restart(int exitCode = 0)
        {
            if (Settings.Default.IsRestarting) return;
            Settings.Default.IsRestarting = true;

            var process = new Process
            {
                StartInfo =
                {
                    FileName = LauncherPath,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = "--delay 2000 --auto-login"
                }
            };
            process.Start();

            Exit(exitCode);
        }

        /// <summary>
        /// Check whether the GUI process has already been started.
        /// </summary>
        public static bool CheckSingleton(string processName, IntPtr windowHandle)
        {
            if (IsDuplicateInstance(processName) && !Settings.Default.IsRestarting)
            {
                windowHandle.ActivateWindow();

                Exit();
                return false;
            }

            Settings.Default.IsRestarting = false;
            return true;
        }

        public static bool IsDuplicateInstance(string processName)
        {
            bool createdNew;
            try
            {
                _keepAliveEvent = new EventWaitHandle(false, EventResetMode.ManualReset, processName, out createdNew);
            }
            catch (UnauthorizedAccessException)
            {
                //The event should exists, ref: https://msdn.microsoft.com/en-us/library/df414y9h(v=vs.110).aspx
                return true;
            }
            catch (Exception e)
            {
                Logger.Fatal("Can't open the communication event. ", e);
                return false;
            }

            if (!createdNew)
            {
                Logger.Fatal("The killed event has been opened. So there is another process has been initialized. ");
                _keepAliveEvent.Close();
                return true;
            }

            //register the exit hook, so that we can raise the name event before 
            return false;
        }

        public static void Exit(int exitCode = 0) => Application.Current?.Shutdown(exitCode);

        public static void Clear()
        {
            //notify the watch dog by setting the event
            if (_keepAliveEvent != null && !_keepAliveEvent.SafeWaitHandle.IsClosed)
            {
                _keepAliveEvent?.Set();
                _keepAliveEvent?.Close();
            }
        }
    }
}

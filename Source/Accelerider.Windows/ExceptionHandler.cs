using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using Accelerider.Windows.Infrastructure;


namespace Accelerider.Windows
{
    public class ExceptionHandler
    {
        private static readonly ILogger Logger = DefaultLogger.Get(typeof(ExceptionHandler));

        private bool _isShowed;

        public void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Log(e.ExceptionObject as Exception);
        }

        public void DispatcherUnhandledExceptionHandler(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Log(e.Exception);
        }

        private void Log(Exception exception)
        {
            Logger.Fatal("An uncaught exception occurred", exception);

            if (_isShowed) return;

            _isShowed = true;
            switch (exception)
            {
                case NotImplementedException _:
                    MessageBox.Show(
                        "Sorry! The feature has NOT been IMPLEMENTED. Please wait for the next version. ",
                        "Fatal",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    break;
                case NotSupportedException _:
                    MessageBox.Show(
                        "Sorry! The feature has NOT been SUPPORTED. Please wait for the next version. ",
                        "Fatal",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    break;
                default:
                    var result = MessageBox.Show(
                        $"Sorry! An uncaught EXCEPTION occurred. {Environment.NewLine}" +
                        $"You can pack and send log files in %AppData%\\Accelerider\\Logs to the developer. Thank you! {Environment.NewLine}{Environment.NewLine}" +
                        $"Do you want to open the Logs folder? ",
                        "Fatal",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Error);

                    if (result == MessageBoxResult.Yes)
                    {
                        Process.Start(AcceleriderFolders.Logs);
                    }

                    break;
            }

            ProcessController.Restart(-1);
        }
    }
}

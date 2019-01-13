using System;
using System.Windows;
using System.Windows.Threading;
using log4net;

namespace Accelerider.Windows
{
    public class ExceptionHandler
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ExceptionHandler));


        public void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Log(e.ExceptionObject as Exception);
        }

        public void DispatcherUnhandledExceptionHandler(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Log(e.Exception);
        }

        private static void Log(Exception exception)
        {
            Logger.Fatal("An uncaught exception occurred", exception);

            switch (exception)
            {
                case NotImplementedException _:
                    MessageBox.Show(
                        "Sorry! The feature has NOT been IMPLEMENTED. Please wait for the next version. ", 
                        "Fatal");
                    break;
                case NotSupportedException _:
                    MessageBox.Show(
                        "Sorry! The feature has NOT been SUPPORTED. Please wait for the next version. ", 
                        "Fatal");
                    break;
                default:
                    MessageBox.Show(
                        "Sorry! An uncaught EXCEPTION occurred. " +
                        "You can pack and send log files in %AppData%\\Accelerider\\Logs to the developer. Thank you! ", 
                        "Fatal");
                    break;
            }

            ProcessController.Restart(-1);
        }
    }
}

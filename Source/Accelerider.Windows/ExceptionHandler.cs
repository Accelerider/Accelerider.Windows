using System;
using System.Windows;
using System.Windows.Threading;
using Prism.Logging;

namespace Accelerider.Windows
{
    public class ExceptionHandler
    {
        private readonly ILoggerFacade _logger;


        public ExceptionHandler(ILoggerFacade logger)
        {
            _logger = logger;
        }

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
            switch (exception)
            {
                case NotImplementedException _:
                    MessageBox.Show("Sorry, The feature has not been implemented, please wait for the next version. ", "Tips");
                    return;
                case NotSupportedException _:
                    MessageBox.Show("Sorry, The feature has not been supported, please wait for the next version. ", "Tips");
                    return;
            }

            var message = $"{exception.GetType().Name}, " +
                          $"Message: {exception.Message}, " +
                          $"StackTrace: {Environment.NewLine}{exception.StackTrace}{Environment.NewLine}";
            _logger.Log(message, Category.Exception, Priority.High);

            Application.Current.Shutdown(-1);
        }
    }
}

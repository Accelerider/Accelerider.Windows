using System;
using System.Windows;
using System.Windows.Threading;
using Autofac;
using Prism.Logging;

namespace Accelerider.Windows
{
    public class ExceptionResolver
    {
        private readonly IContainer _container;
        private readonly ILoggerFacade _logger;


        public ExceptionResolver(IContainer container, ILoggerFacade logger)
        {
            _container = container;
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
            var message = $"{exception.GetType().Name}, " +
                          $"Message: {exception.Message}, " +
                          $"StackTrace: {Environment.NewLine}{exception.StackTrace}{Environment.NewLine}";
            _logger.Log(message, Category.Exception, Priority.High);

            Application.Current.Shutdown(-1);
        }
    }
}

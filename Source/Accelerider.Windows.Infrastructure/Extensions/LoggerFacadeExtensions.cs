using System;

// ReSharper disable once CheckNamespace
namespace Prism.Logging
{
    public static class LoggerFacadeExtensions
    {
        public static void Error(this ILoggerFacade logger, string message, Exception e = null)
        {
            logger.Log(GetMessage(message, e), Category.Exception, Priority.High);
        }

        public static void Warning(this ILoggerFacade logger, string message, Exception e = null)
        {
            logger.Log(GetMessage(message, e), Category.Warn, Priority.Medium);
        }

        public static void Info(this ILoggerFacade logger, string message, Exception e = null)
        {
            logger.Log(GetMessage(message, e), Category.Info, Priority.Low);
        }

        public static void Debug(this ILoggerFacade logger, string message, Exception e = null)
        {
            logger.Log(GetMessage(message, e), Category.Debug, Priority.Low);
        }

        private static string GetMessage(string message, Exception e)
        {
            message = $"{message}{Environment.NewLine}";

            if (e != null)
            {
                message = $"{message}{e.Message}{Environment.NewLine}" +
                          $"{e.StackTrace}{Environment.NewLine}";
            }

            return message;
        }
    }
}

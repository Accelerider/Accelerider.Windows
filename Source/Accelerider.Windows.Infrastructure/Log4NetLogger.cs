using System;
using log4net;
using Prism.Logging;

namespace Accelerider.Windows.Infrastructure
{
    public class Log4NetLogger : ILoggerFacade
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Log4NetLogger));

        public void Log(string message, Category category, Priority priority)
        {
            switch (category)
            {
                case Category.Debug:
                    Logger.Debug(message);
                    break;
                case Category.Exception:
                    Logger.Error(message);
                    break;
                case Category.Info:
                    Logger.Info(message);
                    break;
                case Category.Warn:
                    Logger.Warn(message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(category), category, null);
            }
        }
    }
}

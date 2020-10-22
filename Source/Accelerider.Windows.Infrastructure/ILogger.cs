using System;

namespace Accelerider.Windows.Infrastructure
{
    public interface ILogger
    {
        void Fatal(string message, Exception e = null);

        void Error(string message, Exception e = null);

        void Warning(string message, Exception e = null);

        void Info(string message, Exception e = null);

        void Debug(string message, Exception e = null);
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Media;

namespace Accelerider.Windows.Infrastructure
{
    public class DefaultLogger : ILogger
    {
        private static readonly IDictionary<Type, ILogger> Loggers = new ConcurrentDictionary<Type, ILogger>();

        public static Func<Type, ILogger> LoggerFactory { get; set; }

        public static ILogger Get(Type type)
        {
            if (!Loggers.ContainsKey(type))
            {
                Loggers[type] = LoggerFactory?.Invoke(type) ?? new DefaultLogger(type);
            }

            return Loggers[type];
        }

        private readonly Type _type;

        protected DefaultLogger(Type type) => _type = type;

        public void Fatal(string message, Exception e = null)
        {
            Write(nameof(Fatal), message, e);
        }

        public void Error(string message, Exception e = null)
        {
            Write(nameof(Error), message, e);
        }

        public void Warning(string message, Exception e = null)
        {
            Write(nameof(Warning), message, e);
        }

        public void Info(string message, Exception e = null)
        {
            Write(nameof(Info), message, e);
        }

        public void Debug(string message, Exception e = null)
        {
            Write(nameof(Debug), message, e);
        }

        private void Write(string category, string message, Exception e)
        {
            message = $"{message}{Environment.NewLine}";

            if (e != null)
            {
                message = $"{message}{e.Message}{Environment.NewLine}" +
                          $"{e.StackTrace}{Environment.NewLine}";
            }

            message = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} " +
                      $"[{Thread.CurrentThread.ManagedThreadId}] " +
                      $"{category.ToUpper()} " +
                      $"{_type.FullName}{Environment.NewLine}" +
                      $"{message}{Environment.NewLine}";

            System.Diagnostics.Debug.Write(message);
            File.WriteAllText(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "accelerider.windows.log"),
                message);
        }
    }
}

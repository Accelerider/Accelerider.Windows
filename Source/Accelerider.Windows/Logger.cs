using Prism.Logging;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Accelerider.Windows
{
    public class Logger : ILoggerFacade, IDisposable
    {
        private readonly TextWriter _writer;
        private readonly FileStream _fileStream;
        private readonly string _logFilePath;

        private int _exceptionCount;


        public Logger()
        {
            _logFilePath = GenerateLoggingPath();
            _fileStream = new FileStream(_logFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
            _writer = new StreamWriter(_fileStream, Encoding.UTF8) { AutoFlush = true };
        }


        public void Log(string message, Category category, Priority priority)
        {
            string messageToLog = String.Format(CultureInfo.InvariantCulture, "{1}: {2}. Priority: {3}. Timestamp:{0:u}.",
                DateTime.Now, category.ToString().ToUpper(CultureInfo.InvariantCulture), message, priority.ToString());

            _writer.WriteLine(messageToLog);

            if (category == Category.Exception || category == Category.Warn) _exceptionCount++;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _writer?.Dispose();
                _fileStream?.Dispose();

                if (_exceptionCount==0) File.Delete(_logFilePath);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private string GenerateLoggingPath()
        {
            var directoryPath = $"{Environment.CurrentDirectory}/Logs";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            return $"{directoryPath}/Accelerider.Windows.{DateTime.Now.ToString("yyyyMMddHHmmssff")}.log";
        }
    }
}

using System;

namespace BaiduPanDownloadWpf.Core.NetWork.Exceptions
{
    class ServerException : Exception
    {
        public ServerException() : base()
        {
        }

        public ServerException(string message) : base(message)
        {
        }

        public ServerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

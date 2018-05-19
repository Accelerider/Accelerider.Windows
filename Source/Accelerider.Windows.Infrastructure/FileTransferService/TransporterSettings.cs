using System.Net.Http.Headers;

namespace Accelerider.Windows.Infrastructure.FileTransferService
{
    public class TransporterSettings
    {
        public HttpHeaders Headers { get; set; }

        public int MaxErrorCount { get; set; }

        /// <summary>
        /// If some uri speed is faster than current uri, this option can auto switch into that uri.
        /// </summary>
        public bool AutoSwitchUri { get; set; }

        public int ConnectTimeout { get; set; } = 1000 * 30; //30S

        public int ReadWriteTimeout { get; set; } = 1000 * 30; //30S

        public int ThreadCount { get; set; } = 16;

        public DataSize BlockSize { get; set; } = 1024 * 1024 * 10L; //10MB

        public string LinkHandlerType { get; set; }

        public string LinkGetterInfo { get; set; }

        public void CopyTo(TransporterSettings settings)
        {
            settings.AutoSwitchUri = AutoSwitchUri;
            settings.BlockSize = BlockSize;
            settings.ConnectTimeout = ConnectTimeout;
            settings.Headers = Headers;
            settings.LinkGetterInfo = LinkGetterInfo;
            settings.LinkHandlerType = LinkHandlerType;
            settings.MaxErrorCount = MaxErrorCount;
            settings.ReadWriteTimeout = ReadWriteTimeout;
            settings.ThreadCount = ThreadCount;
        }
    }
}

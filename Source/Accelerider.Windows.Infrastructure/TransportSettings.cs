using System.Net.Http.Headers;

namespace Accelerider.Windows.Infrastructure
{
    public class TransportSettings
    {
        public HttpHeaders Headers { get; set; }

        public int MaxErrorCount { get; set; }

        /// <summary>
        /// If some uri speed is relatively faster, this option can auto switch into that uri.
        /// </summary>
        public bool AutoSwitchUri { get; set; }

        public int ConnectTimeout { get; set; } = 1000 * 30; //30S

        public int ReadWriteTimeout { get; set; } = 1000 * 30; //30S

        public int ThreadCount { get; set; } = 16;

        public long BlockSize { get; set; } = 1024 * 1024 * 4L; //4MB


        internal TransportSettings() { }
    }
}

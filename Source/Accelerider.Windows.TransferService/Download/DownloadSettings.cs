using Polly;

namespace Accelerider.Windows.TransferService
{
    public class DownloadSettings
    {
        internal DownloadSettings() { }

        public IAsyncPolicy BuildPolicy { get; set; }

        public BlockDownloadItemPolicy DownloadPolicy { get; internal set; }

        public int MaxConcurrent { get; set; }
    }
}

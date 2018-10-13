using System;

namespace Accelerider.Windows.Infrastructure.TransferService.Wpf
{
    public class DownloaderBlockItemBindable
    {
        public Guid Id { get; }

        public ProgressBindable Progress { get; }

        public DownloaderBlockItemBindable(Guid id, long sampleIntervalBasedMilliseconds)
        {
            Id = id;
            Progress = new ProgressBindable(sampleIntervalBasedMilliseconds);
        }
    }
}

using System;

namespace Accelerider.Windows.Infrastructure.WpfInteractions
{
    public class BindableBlockDownloadItem
    {
        public Guid Id { get; }

        public BindableProgress Progress { get; }

        public BindableBlockDownloadItem(Guid id, long sampleIntervalBasedMilliseconds)
        {
            Id = id;
            Progress = new BindableProgress(sampleIntervalBasedMilliseconds);
        }
    }
}

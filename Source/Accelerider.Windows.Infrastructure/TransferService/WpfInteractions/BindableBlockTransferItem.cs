using System;

namespace Accelerider.Windows.Infrastructure.WpfInteractions
{
    public class BindableBlockTransferItem
    {
        public Guid Id { get; }

        public BindableProgress Progress { get; }

        public BindableBlockTransferItem(Guid id, long sampleIntervalBasedMilliseconds)
        {
            Id = id;
            Progress = new BindableProgress(sampleIntervalBasedMilliseconds);
        }
    }
}

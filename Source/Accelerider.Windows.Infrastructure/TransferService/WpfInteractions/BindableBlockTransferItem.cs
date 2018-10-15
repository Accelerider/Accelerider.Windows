using System;

namespace Accelerider.Windows.Infrastructure.TransferService.WpfInteractions
{
    public class BindableBlockTransferItem
    {
        public Guid Id { get; }

        public BindableProgress Progress { get; }

        public BindableBlockTransferItem(Guid id)
        {
            Id = id;
            Progress = new BindableProgress();
        }
    }
}

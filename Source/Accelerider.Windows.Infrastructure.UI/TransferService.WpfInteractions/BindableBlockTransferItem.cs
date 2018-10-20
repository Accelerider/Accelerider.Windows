using System;

// ReSharper disable once CheckNamespace
namespace Accelerider.Windows.TransferService.WpfInteractions
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

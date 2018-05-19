using System;

namespace Accelerider.Windows.Infrastructure.FileTransferService
{
    public class TransferStatusChangedEventArgs : EventArgs
    {
        public TransferStatusChangedEventArgs(TransferStatus oldStatus, TransferStatus newStatus)
        {
            OldStatus = oldStatus;
            NewStatus = newStatus;
        }

        public TransferStatus OldStatus { get; }

        public TransferStatus NewStatus { get; }
    }
}

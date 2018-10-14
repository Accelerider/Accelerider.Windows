using System;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    public struct TransferNotification
    {
        public TransferNotification(Guid currentBlockId, TransferStatus status, long bytes)
        {
            CurrentBlockId = currentBlockId;
            Status = status;
            Bytes = bytes;
        }

        public Guid CurrentBlockId { get; }

        public TransferStatus Status { get; }

        public long Bytes { get; }
    }
}

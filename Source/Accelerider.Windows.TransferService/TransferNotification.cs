namespace Accelerider.Windows.TransferService
{
    public struct TransferNotification
    {
        public TransferNotification(long offset, TransferStatus status, long bytes)
        {
            Offset = offset;
            Status = status;
            Bytes = bytes;
        }

        public long Offset { get; }

        public TransferStatus Status { get; }

        public long Bytes { get; }
    }
}

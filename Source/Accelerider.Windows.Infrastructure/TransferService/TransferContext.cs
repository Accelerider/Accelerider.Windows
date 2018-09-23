using System;
using Accelerider.Windows.Infrastructure.FileTransferService;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    public class TransferContext
    {
        public Guid Id { get; } = Guid.NewGuid();

        public TransferStatus Status { get; internal set; }

        public long CompletedSize { get; internal set; }

        public long TotalSize { get; internal set; }

        public string LocalPath { get; internal set; }

        public string RemotePath { get; internal set; }
    }

    public class BlockTransferContext : TransferContext
    {
        private int _bytes;

        public long Offset { get; internal set; }

        public int Bytes
        {
            get => _bytes;
            internal set
            {
                _bytes = value;
                CompletedSize += value;
            }
        }
    }
}

using System;
//using Accelerider.Windows.Infrastructure.FileTransferService;
using Newtonsoft.Json;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    public class TransferContextBase
    {
        public Guid Id { get; } = Guid.NewGuid();

        public long TotalSize { get; internal set; }

        public string LocalPath { get; internal set; }
    }

    public class TransferContext : TransferContextBase
    {
        public IRemotePathProvider RemotePathProvider { get; internal set; }
    }

    public class BlockTransferContext : TransferContextBase
    {
        private int _bytes;

        public long Offset { get; internal set; }

        public long CompletedSize { get; internal set; }

        [JsonIgnore]
        public int Bytes
        {
            get => _bytes;
            internal set
            {
                _bytes = value;
                CompletedSize += value;
            }
        }

        public string RemotePath { get; internal set; }
    }
}

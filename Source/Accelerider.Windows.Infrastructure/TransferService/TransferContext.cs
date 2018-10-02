using System;
using Newtonsoft.Json;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    public class TransferContextBase
    {
        [JsonProperty]
        public Guid Id { get; private set; } = Guid.NewGuid();

        [JsonProperty]
        public long TotalSize { get; internal set; }

        [JsonProperty]
        public virtual string LocalPath { get; internal set; }
    }

    public class DownloadContext : TransferContextBase
    {
        [JsonProperty]
        public IRemotePathProvider RemotePathProvider { get; internal set; }
    }

    public class BlockTransferContext : TransferContextBase
    {
        [JsonProperty]
        public long Offset { get; internal set; }

        [JsonProperty]
        public long CompletedSize { get; internal set; }

        [JsonIgnore]
        public override string LocalPath { get; internal set; }

        [JsonProperty]
        public string RemotePath { get; internal set; }
    }

    public struct DownloaderNotification
    {
        public DownloaderNotification(Guid currentBlockId, TransferStatus status, long bytes)
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

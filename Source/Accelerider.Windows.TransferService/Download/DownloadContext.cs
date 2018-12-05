using System;
using Newtonsoft.Json;

namespace Accelerider.Windows.TransferService
{
    public class DownloadContext
    {
        [JsonProperty]
        public Guid Id { get; private set; }

        [JsonProperty]
        public long TotalSize { get; internal set; }

        [JsonProperty]
        public string LocalPath { get; internal set; }

        [JsonProperty]
        public IRemotePathProvider RemotePathProvider { get; internal set; }

        public DownloadContext(Guid id) => Id = id;
    }
}

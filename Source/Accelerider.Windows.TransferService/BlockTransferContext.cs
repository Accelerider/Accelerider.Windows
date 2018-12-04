using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Accelerider.Windows.TransferService
{
    public class BlockTransferContext
    {
        public const long InvalidOffset = -1;

        [JsonProperty]
        public long Offset { get; internal set; }

        [JsonProperty]
        public long CompletedSize { get; internal set; }

        [JsonProperty]
        public long TotalSize { get; internal set; }

        [JsonIgnore]
        public string LocalPath { get; internal set; }

        [JsonIgnore]
        public Func<Task<string>> RemotePathGetter { get; internal set; }
    }
}

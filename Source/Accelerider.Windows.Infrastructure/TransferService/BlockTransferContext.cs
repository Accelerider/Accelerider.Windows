using System;
using Newtonsoft.Json;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    public class BlockTransferContext : TransferContextBase
    {
        public BlockTransferContext()
        {
            if (Id == default) Id = Guid.NewGuid();
        }

        [JsonProperty]
        public long Offset { get; internal set; }

        [JsonProperty]
        public long CompletedSize { get; internal set; }

        [JsonIgnore]
        public override string LocalPath { get; internal set; }

        [JsonProperty]
        public string RemotePath { get; internal set; }
    }
}

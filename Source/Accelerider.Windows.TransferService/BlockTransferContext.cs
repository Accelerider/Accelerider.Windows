using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Accelerider.Windows.TransferService
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

        [JsonIgnore]
        public Func<Task<string>> RemotePathGetter { get; internal set; }
    }
}

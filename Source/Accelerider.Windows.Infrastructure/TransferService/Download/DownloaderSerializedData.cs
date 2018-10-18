using System.Collections.Generic;
using Newtonsoft.Json;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    internal class DownloaderSerializedData
    {
        [JsonProperty]
        public object Tag { get; internal set; }

        [JsonProperty]
        public DownloadContext Context { get; internal set; }

        [JsonProperty]
        public List<BlockTransferContext> BlockContexts { get; internal set; }
    }
}

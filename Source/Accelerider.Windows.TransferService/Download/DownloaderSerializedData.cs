using System.Collections.Generic;
using Newtonsoft.Json;

namespace Accelerider.Windows.TransferService
{
    internal class DownloaderSerializedData
    {
        [JsonProperty]
        public DownloadContext Context { get; internal set; }

        [JsonProperty]
        public List<BlockTransferContext> BlockContexts { get; internal set; }
    }
}

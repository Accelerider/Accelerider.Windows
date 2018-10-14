using System;
using Newtonsoft.Json;


namespace Accelerider.Windows.Infrastructure.TransferService
{
    public class DownloadContext : TransferContextBase
    {
        public DownloadContext(Guid id) => Id = id;

        [JsonProperty]
        public IRemotePathProvider RemotePathProvider { get; internal set; }
    }
}

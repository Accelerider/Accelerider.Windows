using System;
using System.Threading.Tasks;
using Accelerider.Windows.TransferService;
using Newtonsoft.Json;

namespace Accelerider.Windows.Modules.NetDisk.Models.SixCloud
{
    public class SixCloudRemotePathProvider : IRemotePathProvider
    {
        [JsonProperty("file")]
        private SixCloudFile _file;

        public SixCloudRemotePathProvider(SixCloudFile file) => _file = file;

        public void Rate(string remotePath, double score) => throw new NotSupportedException();

        public async Task<string> GetRemotePathAsync() => await _file.GetDownloadAddressAsync();
    }
}

using System;
using System.Threading.Tasks;
using Accelerider.Windows.TransferService;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Accelerider.Windows.Modules.NetDisk.Models.SixCloud
{
    public class SixCloudRemotePathProvider : IRemotePathProvider
    {
        private const string File = "file";

        [JsonProperty(File)]
        private SixCloudFile _file;

        public SixCloudRemotePathProvider(SixCloudFile file) => _file = file;

        public SixCloudRemotePathProvider(string json, SixCloudUser owner)
        {
            var fileJToken = JObject.Parse(json)[File];
            _file = fileJToken.ToObject<SixCloudFile>();
            _file.Owner = owner;
        }

        public void Rate(string remotePath, double score) { /*NotSupportedException*/ }

        public async Task<string> GetRemotePathAsync() => await _file.GetDownloadAddressAsync();
    }
}

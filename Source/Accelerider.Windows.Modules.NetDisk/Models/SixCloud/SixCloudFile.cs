using System;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Modules.NetDisk.Enumerations;
using Newtonsoft.Json;

namespace Accelerider.Windows.Modules.NetDisk.Models.SixCloud
{
    public class SixCloudFile : INetDiskFile
    {
        #region Private

        [JsonProperty("uuid")]
        private string _uuid;
        [JsonProperty("name")]
        private string _name;
        [JsonProperty("ext")]
        private string _ext;
        [JsonProperty("type")]
        private int _type;
        [JsonProperty("size")]
        private long _size;
        [JsonProperty("ctime")]
        private long _ctime;
        [JsonProperty("path")]
        private string _path;

        #endregion

        public FileType Type => _type == 1 ? FileType.FolderType : Path.GetFileType();

        public FileLocator Path => string.IsNullOrEmpty(_path) ? "/" : _path;

        public long Size => _size;

        public SixCloudUser Owner { get; set; }

        public DateTime ModifiedTime => new DateTime(1970, 1, 1) + TimeSpan.FromMilliseconds(_ctime);

        public async Task<string> GetDownloadAddressAsync() =>
            (await Owner.Api.GetFileInfoByPathAsync(new PathArgs { Path = Path })).Result["downloadAddress"]
            .ToObject<string>();
    }
}

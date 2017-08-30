using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Core.NetWork;
using Accelerider.Windows.Core.Tools;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Accelerider.Windows.Core.Files.AcceleriderCloud
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class AcceleriderCloudFile : DiskFileBase, INetDiskFile
    {
        [JsonProperty("fileName")]
        private string _fileName;

        [JsonProperty("path")]
        private string _path;

        [JsonProperty("size")]
        private long _size;

        [JsonProperty("dir")]
        private int _dir = 1;

        [JsonProperty("encryption")]
        private int _encryption;

        [JsonProperty("md5")]
        private string _md5;

        [JsonProperty("ctime")]
        private long _ctime;

        internal AcceleriderUser User { get; set; }

        public new FileTypeEnum FileType => _dir == 1
            ? FileTypeEnum.FolderType
            : (from item in FileTypeDirectory
                where item.Value.Contains(FilePath.FileExtension)
                select item.Key).SingleOrDefault();

        public override async Task<bool> DeleteAsync()
        {
            
            return
                JObject.Parse(
                        await
                            new HttpClient().GetAsync(
                                $"http://api.usmusic.cn/cloud/delete?token={User.Token}&path={_path.UrlEncode()}"))
                    .Value<int>("errno") == 0;
        }
        public new FileLocation FilePath => new FileLocation(string.IsNullOrEmpty(_path) ? "/" : _path);
        public new DataSize FileSize => new DataSize(_size);

        internal string FileName => _fileName;
        public DateTime ModifiedTime => new DateTime(1970, 1, 1, 8, 0, 0) + TimeSpan.FromSeconds(_ctime);
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Core.NetWork;
using Accelerider.Windows.Core.NetWork.UserModels;
using Accelerider.Windows.Core.Tools;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Accelerider.Windows.Core.Files.OneDrive
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class OneDriveFile : DiskFileBase, INetDiskFile
    {
        [JsonProperty("id")]
        internal string Id { get; set; }

        [JsonProperty("fileName")]
        internal string FileName { get; set; }

        [JsonProperty("downloadLink")]
        internal string DownloadLink { get; set; }

        [JsonProperty("createTime")]
        private string _createTime;

        [JsonProperty("size")]
        private long _size;

        [JsonProperty("path")]
        private string _path;

        [JsonProperty("dir")]
        private int _dir = 1;

        internal OneDriveUser User { get; set; }

        

        public new FileTypeEnum FileType => _dir == 1
            ? FileTypeEnum.FolderType
            : (from item in FileTypeDirectory
                where item.Value.Contains(FilePath.FileExtension)
                select item.Key).SingleOrDefault();


        public new FileLocation FilePath => new FileLocation(string.IsNullOrEmpty(_path) ? "/" : _path);
        public override async Task<bool> DeleteAsync()
        {
            return JObject.Parse(await
                           new HttpClient().GetAsync(
                               $"http://api.usmusic.cn/onedrive/delete?token={User.AccUser.Token}&user={User.Userid}&path={_path.UrlEncode()}"))
                       .Value<int>("errno") == 0;
        }
        public new DataSize FileSize => new DataSize(_size);
        public DateTime ModifiedTime => DateTime.Parse(_createTime);
    }
}

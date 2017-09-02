using System;
using System.Collections.Generic;
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

namespace Accelerider.Windows.Core.Files.BaiduNetDisk
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class BaiduNetDiskFile : DiskFileBase, INetDiskFile
    {
        [JsonProperty("fs_id")]
        private long _fileId;
        [JsonProperty("server_ctime")]
        private long _serverCtime;
        [JsonProperty("server_mtime")]
        private long _serverMtime;
        [JsonProperty("local_ctime")]
        private long _localCtime;
        [JsonProperty("local_mtime")]
        private long _localMtime;
        [JsonProperty("size")]
        private long _size;
        [JsonProperty("isdir")]
        private int _isDir = 1;
        [JsonProperty("dir_empty")]
        private int _dirEmpty;
        [JsonProperty("path")]
        private string _path;
        [JsonProperty("server_filename")]
        private string _serverFileName;
        [JsonProperty("empty")]
        private int _empty;
        [JsonProperty("md5")]
        private string _md5;

        internal BaiduNetDiskUser User { get; set; }


        public override FileTypeEnum FileType => _isDir == 1 ? FileTypeEnum.FolderType : (from item in FileTypeDirectory
                                                                                          where item.Value.Contains(FilePath.FileExtension)
                                                                                          select item.Key).SingleOrDefault();

        public override FileLocation FilePath => string.IsNullOrEmpty(_path) ? "/" : _path;

        public override DataSize FileSize => new DataSize(_size);


        public DateTime ModifiedTime => new DateTime(1970, 1, 1, 8, 0, 0) + TimeSpan.FromSeconds(_serverMtime);


        internal string FileName => _serverFileName;


        public override async Task<bool> DeleteAsync()
        {
            var json = JObject.Parse(await new HttpClient().GetAsync($"http://api.usmusic.cn/deleteFile?token={User.AccUser.Token}&uk={User.Userid}&path={_path.UrlEncode()}"));
            return json.Value<int>("errno") == 0;
        }
    }
}

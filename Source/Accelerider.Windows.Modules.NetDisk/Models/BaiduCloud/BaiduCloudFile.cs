using System;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Modules.NetDisk.Enumerations;
using Newtonsoft.Json;

namespace Accelerider.Windows.Modules.NetDisk.Models.BaiduCloud
{
    public class BaiduCloudFile : INetDiskFile
    {
        [JsonProperty("path")]
        private string _path;

        public FileType Type => _isDir == 1 ? FileType.FolderType : FileType.OtherType;

        public FileLocator Path => string.IsNullOrEmpty(_path) ? "/" : _path;

        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("fs_id")]
        public long FileId { get; set; }

        [JsonProperty("server_filename")]
        public string FileName { get; set; }

        [JsonProperty("md5")]
        public string ServerMd5 { get; set; }


        public DateTime ModifiedTime => new DateTime(1970, 1, 1) + TimeSpan.FromSeconds(_mtime);

        [JsonProperty("isdir")]
        private int _isDir = 1;

        //public BaiduCloudUser Owner { get; set; }

        [JsonProperty("server_mtime")]
        private long _mtime;


        //public async Task<bool> DeleteAsync() => (await Owner.Api.DeleteFileAsync(Owner.Token, StringExtensions.LogId,
        //                                             new Dictionary<string, string> { ["filelist"] = $"[\"{Path}\"]" }))
        //                                         .ErrorCode == 0;

    }
}

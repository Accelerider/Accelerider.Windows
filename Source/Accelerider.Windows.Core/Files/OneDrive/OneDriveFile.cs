using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Core.NetWork.UserModels;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;
using Newtonsoft.Json;

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
        public override Task<bool> DeleteAsync()
        {
            throw new NotImplementedException();
        }
        public new DataSize FileSize => new DataSize(_size);
        public DateTime ModifiedTime => DateTime.Parse(_createTime);
    }
}

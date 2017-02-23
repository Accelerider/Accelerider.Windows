using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using BaiduPanDownloadWpf.Infrastructure;
using BaiduPanDownloadWpf.Infrastructure.Interfaces;
using Newtonsoft.Json;
using System.Threading.Tasks;
using BaiduPanDownloadWpf.Core.NetWork.Packets;
using BaiduPanDownloadWpf.Infrastructure.Interfaces.Files;
using Newtonsoft.Json.Linq;

namespace BaiduPanDownloadWpf.Core
{
    public class NetDiskFile : INetDiskFile
    {
        #region Private fileds
        private NetDiskUser _netDiskUser;

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

        private FileLocation _filePath;
        private DateTime? _createTime;
        private DateTime? _motifyTime;
        private FileTypeEnum? _fileType;
        #region FileTypeDirectory
        public static readonly Dictionary<FileTypeEnum, string[]> FileTypeDirectory = new Dictionary<FileTypeEnum, string[]>()
        {
            { FileTypeEnum.ApkType, new []{ "apk" }},
            { FileTypeEnum.DocType, new []{ "doc", "docx" } },
            { FileTypeEnum.ExeType, new []{ "exe" } },
            { FileTypeEnum.ImgType, new []{ "png", "jpg", "jpeg", "bmp", "git" } },
            { FileTypeEnum.MixFileType, new []{ "mix" } },
            { FileTypeEnum.MusicType, new []{ "mp3", "wav", "aac", "wma", "flac", "ape", "ogg" } },
            { FileTypeEnum.PdfType, new []{ "pdf" } },
            { FileTypeEnum.PptType, new []{ "ppt", "pptx" } },
            { FileTypeEnum.RarType, new []{ "rar", "zip", "7z", "iso" } },
            { FileTypeEnum.TorrentType, new []{ "torrent" } },
            { FileTypeEnum.TxtType, new []{ "txt", "lrc" } },
            { FileTypeEnum.VideoType, new []{ "rmvb", "mp4", "avi", "rm", "wmv", "flv", "f4v", "mkv", "3gp" } },
            { FileTypeEnum.XlsType, new []{ "xls", "xlsx" } },
        };
        #endregion
        #endregion

        /// <summary>
        /// Intializes an instance of the <see cref="NetDiskFile"/> by specified an instance of the <see cref="INetDiskFile"/>,
        /// which will serve as the root file for the <see cref="INetDiskUser"/> instance.
        /// </summary>
        /// <param name="netDiskUser">The owner of the instance.</param>
        public NetDiskFile(NetDiskUser netDiskUser)
        {
            _netDiskUser = netDiskUser;
            _path = "/";
        }

        #region Public properties
        /// <summary>
        /// 文件夹是否为空
        /// </summary>
        [JsonIgnore]
        public bool IsDirEmpty => _dirEmpty == 1;

        /// <summary>
        /// 文件在服务器上的创建时间
        /// </summary>
        [JsonIgnore]
        public long CreateTimeServer => _serverCtime;

        /// <summary>
        /// 文件在服务器上的修改时间
        /// </summary>
        [JsonIgnore]
        public long ModifyTimeServer => _serverMtime;

        /// <summary>
        /// 文件本身的创建时间
        /// </summary>
        [JsonIgnore]
        public long CreateTimeLocal => _localCtime;

        /// <summary>
        /// 文件本身的修改时间
        /// </summary>
        [JsonIgnore]
        public long ModifyTimeLocal => _localMtime;

        /// <summary>
        /// 文件md5
        /// </summary>
        [JsonIgnore]
        public string Md5 => _md5;

        /// <summary>
        /// 文件是否为空
        /// </summary>
        [JsonIgnore]
        public bool Empty => _empty == 1;

        /// <summary>
        /// Gets the ID of the file.
        /// </summary>
        [JsonIgnore]
        public long FileId => _fileId;

        /// <summary>
        /// Gets the time at which the file was created.
        /// </summary>
        [JsonIgnore]
        public DateTime CreatedTime => _createTime ?? (_createTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddSeconds(_serverCtime)).Value;

        /// <summary>
        /// Gets the time at which the file was motified.
        /// </summary>
        [JsonIgnore]
        public DateTime MotifiedTime => _motifyTime ?? (_motifyTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddSeconds(_serverMtime)).Value;

        /// <summary>
        /// Gets a instance of <see cref="FileLocation"/> which contains the file location information.
        /// </summary>
        [JsonIgnore]
        public FileLocation FilePath => _filePath ?? (_filePath = new FileLocation(_path));

        /// <summary>
        /// Gets the type of the file.
        /// </summary>
        [JsonIgnore]
        public FileTypeEnum FileType => _fileType ?? (_fileType = ParseFileType(FilePath.FileExtension)).Value;

        /// <summary>
        /// Gets the size of the file.
        /// </summary>
        [JsonIgnore]
        public long FileSize => _size;
        #endregion

        /// <summary>
        /// Gets children file of the current file by async.
        /// </summary>
        /// <returns>A instance of <see cref="IEnumerable{T}"/> contains the children file of the current file.</returns>
        public async Task<IEnumerable<INetDiskFile>> GetChildrenAsync()
        {
            if (_isDir == 0 || _netDiskUser == null) return null;

            var json = JObject.Parse(await _netDiskUser.DataServer.SendPacketAsync(new ListFilePacket
            {
                Token = _netDiskUser.Token,
                Path = FilePath.FullPath
            }));
            var result = json["list"]?.Select(job => JsonConvert.DeserializeObject<NetDiskFile>(job.ToString()));
            if (result == null) throw new NullReferenceException("目标目录不存在");
            var diskFiles = result as NetDiskFile[] ?? result.ToArray();
            foreach (var item in diskFiles)
            {
                item._netDiskUser = _netDiskUser;
            }
            return diskFiles;
        }

        public async Task DownloadAsync()
        {
            var path = await GetDownloadPathAndFile(_netDiskUser.DownloadDirectory);
            foreach (var item in path)
            {
                _netDiskUser.LocalUser.DownloadFile(item.Value, item.Key);
            }
        }

        /// <summary>
        /// 获取下载目录对应的文件(保留文件结构)
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, NetDiskFile>> GetDownloadPathAndFile(string path)
        {
            var ret = new Dictionary<string, NetDiskFile>();
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            if (_isDir != 1)
            {
                ret.Add(Path.Combine(path, _serverFileName), this);
                return ret;
            }
            foreach (var childrenFile in await GetChildrenAsync())
            {
                foreach (var sub in await ((NetDiskFile)childrenFile).GetDownloadPathAndFile(Path.Combine(path, _serverFileName)))
                {
                    ret.Add(sub.Key, sub.Value);
                }
            }
            return ret;
        }

        public Task<bool> DeleteAsync()
        {
            
            return Task.Run(async () =>
            {
                var json = await _netDiskUser.DataServer.SendPacketAsync(new DeleteFilePacket()
                {
                    Token = _netDiskUser.Token,
                    Path = _path
                });
                return (int)JObject.Parse(json)["errno"] == 0;
            });
            
            /*
            return Task.Run(async () =>
            {
                var json = await _netDiskUser.DataServer.SendPacketAsync(new CreateLinkPacket()
                {
                    Token = _netDiskUser.Token,
                    Info = this
                });
                Console.WriteLine(json);
                return true;
                return (int)JObject.Parse(json)["errno"] == 0;
            });
            */
        }

        public Task<bool> RestoreAsync()
        {
            throw new NotImplementedException();
        }

        private FileTypeEnum ParseFileType(string fileExtension)
        {
            return _isDir == 1 ? FileTypeEnum.FolderType : (from item in FileTypeDirectory where item.Value.Contains(fileExtension) select item.Key).FirstOrDefault();
        }
    }
}

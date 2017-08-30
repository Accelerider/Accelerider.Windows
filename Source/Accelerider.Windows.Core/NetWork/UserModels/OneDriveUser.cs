using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Core.DownloadEngine;
using Accelerider.Windows.Core.Files.OneDrive;
using Accelerider.Windows.Core.Tools;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Accelerider.Windows.Core.NetWork.UserModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class OneDriveUser : IOneDriveUser, ITaskCreator
    {
        [JsonProperty("name")]
        public string Username { get; set; }
        public DataSize TotalCapacity => new DataSize(_totalQuota);
        public DataSize UsedCapacity => new DataSize(_usedQuota);

        [JsonProperty("id")]
        public string Userid { get; set; }

        [JsonProperty("totalQuota")]
        private long _totalQuota;

        [JsonProperty("usedQuota")]
        private long _usedQuota;

        internal AcceleriderUser AccUser { get; set; }


        public async Task<bool> RefreshUserInfoAsync()
        {
            return true;
        }

        public ITransferTaskToken UploadAsync(FileLocation from, FileLocation to)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyCollection<ITransferTaskToken>> DownloadAsync(ILazyTreeNode<INetDiskFile> fileNode, FileLocation downloadFolder = null)
        {
            var path = downloadFolder.FullPath;
            var filelist = new List<INetDiskFile>();

            await fileNode.ForEachAsync(file => filelist.Add(file));
            var temp = filelist.Where(v => v.FileType != FileTypeEnum.FolderType)
                .Select(v => (File: v, DownloadPath: path + (path.Split('\\').Length == 2 ? string.Empty : @"\") + v
                                                         .FilePath.FullPath
                                                         .Replace(fileNode.Content.FilePath.FullPath + "/",
                                                             string.Empty)
                                                         .Replace("/", @"\")));
            return temp.Select(v => DownloadTaskManager.Manager.Add(new DownloadTaskItem()
            {
                FilePath = v.File.FilePath.FullPath,
                DownloadPath = v.DownloadPath,
                FromUser = Userid,
                Completed = false
            })).ToList();
        }

        /*
        private async Task<IReadOnlyCollection<OneDriveFile>> GetFilesByPath(string path)
        {
            var json = JObject.Parse(
                await new HttpClient().GetAsync(
                    $"http://api.usmusic.cn/onedrive/filelist?token={AccUser.Token}&user={Userid}&path={path.UrlEncode()}"));
            if (json.Value<int>("errno") != 0) return null;
            return JArray.Parse(json["list"].ToString()).Select(v =>
            {
                var file = JsonConvert.DeserializeObject<OneDriveFile>(v.ToString());
                file.User = this;
                return file;
            }).ToList();
        }
        */
        public Task<(ShareStateCode, ISharedFile)> ShareAsync(IEnumerable<INetDiskFile> files, string password = null)
        {
            throw new NotImplementedException();
        }

        public async Task<ILazyTreeNode<INetDiskFile>> GetNetDiskFileRootAsync()
        {
            await Task.Delay(100);
            var tree = new LazyTreeNode<INetDiskFile>(new OneDriveFile { User = this })
            {
                ChildrenProvider = async parent =>
                {
                    if (parent.FileType != FileTypeEnum.FolderType) return null;
                    var json = JObject.Parse(
                        await new HttpClient().GetAsync(
                            $"http://api.usmusic.cn/onedrive/filelist?token={AccUser.Token}&user={Userid}&path={parent.FilePath.FullPath.UrlEncode()}"));
                    if (json.Value<int>("errno") != 0) return null;
                    return JArray.Parse(json["list"].ToString()).Select(v =>
                    {
                        var file = JsonConvert.DeserializeObject<OneDriveFile>(v.ToString());
                        file.User = this;
                        return file;
                    });
                }
            };
            return tree;
        }

        public Task<IEnumerable<ISharedFile>> GetSharedFilesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IDeletedFile>> GetDeletedFilesAsync()
        {
            throw new NotImplementedException();
        }


        public async Task<IReadOnlyCollection<string>> GetDownloadUrls(string file)
        {
            return await Task.Run(() => new[] { (GetNetDiskFileByPath(file) as OneDriveFile)?.DownloadLink });

        }

        public INetDiskFile GetNetDiskFileByPath(string path)
        {
            var fileName = path.Split('/').Last();
            var json = JObject.Parse(new HttpClient().Get(
                    $"http://api.usmusic.cn/onedrive/filelist?token={AccUser.Token}&user={Userid}&path={path.GetSuperPath().UrlEncode()}"));
            if (json.Value<int>("errno") != 0) return null;
            return JArray.Parse(json["list"].ToString()).Select(v =>
            {
                var file = JsonConvert.DeserializeObject<OneDriveFile>(v.ToString());
                file.User = this;
                return file;
            }).FirstOrDefault(v => v.FileName == fileName);
        }
    }
}

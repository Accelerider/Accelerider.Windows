using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Core.DownloadEngine;
using Accelerider.Windows.Core.Files.BaiduNetDisk;
using Accelerider.Windows.Core.Tools;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Accelerider.Windows.Core.NetWork.UserModels
{
    internal class BaiduNetDiskUser : IBaiduCloudUser, ITaskCreator
    {
        public Uri HeadImageUri { get; set; }
        public string Username { get; set; }
        public string Nickname { get; set; }
        public DataSize TotalCapacity { get; set; }
        public DataSize UsedCapacity { get; set; }
        public string Userid { get; }

        public AcceleriderUser AccUser { get; }

        internal BaiduNetDiskUser(AcceleriderUser user, string userid)
        {
            AccUser = user;
            Userid = userid;
        }

        public ITransferTaskToken UploadAsync(FileLocation from, FileLocation to)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyCollection<ITransferTaskToken>> DownloadAsync(ILazyTreeNode<INetDiskFile> fileNode, FileLocation downloadFolder = null)
        {
            var result = new List<ITransferTaskToken>();
            if (fileNode.Content.FileType == FileTypeEnum.FolderType)
            {
                var redundantPathLength = fileNode.Content.FilePath.FolderPath.Length + 1;
                await fileNode.ForEachAsync(file =>
                {
                    if (file.FileType == FileTypeEnum.FolderType) return;

                    var subPath = file.FilePath.FullPath.Substring(redundantPathLength);
                    FileLocation downloadPath = Path.Combine(downloadFolder, subPath);
                    if (!Directory.Exists(downloadPath.FolderPath))
                        Directory.CreateDirectory(downloadPath.FolderPath);
                    result.Add(DownloadTaskManager.Manager.Add(new DownloadTaskItem()
                    {
                        FilePath = file.FilePath,
                        DownloadPath = downloadPath,
                        FromUser = Userid,
                        Completed = false
                    }));
                });
            }
            else
            {
                result.Add(DownloadTaskManager.Manager.Add(new DownloadTaskItem()
                {
                    FilePath = fileNode.Content.FilePath,
                    DownloadPath = Path.Combine(downloadFolder, fileNode.Content.FilePath.FileName),
                    FromUser = Userid,
                    Completed = false
                }));
            }
            return result;
        }

        public Task<(ShareStateCode, ISharedFile)> ShareAsync(IEnumerable<INetDiskFile> files, string password = null)
        {
            throw new NotImplementedException();
        }

        public async Task<ILazyTreeNode<INetDiskFile>> GetNetDiskFileRootAsync()
        {
            await Task.Delay(100);
            var tree = new LazyTreeNode<INetDiskFile>(new BaiduNetDiskFile { User = this })
            {
                ChildrenProvider = async parent =>
                {
                    if (parent.FileType != FileTypeEnum.FolderType) return null;
                    var json = JObject.Parse(await new HttpClient().GetAsync($"http://api.usmusic.cn/filelist?token={AccUser.Token}&uk={Userid}&path={parent.FilePath.FullPath.UrlEncode()}"));
                    if (json.Value<int>("errno") != 0) return null;
                    return JArray.Parse(json["list"].ToString()).Select(v =>
                    {
                        var file = JsonConvert.DeserializeObject<BaiduNetDiskFile>(v.ToString());
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

        public async Task<bool> RefreshUserInfoAsync()
        {
            var json = JObject.Parse(await
                new HttpClient().GetAsync($"http://api.usmusic.cn/userinfo?token={AccUser.Token}&uk={Userid}"));
            if (json.Value<int>("errno") == 0)
            {
                HeadImageUri = new Uri(json.Value<string>("avatar_url"));
                Username = json.Value<string>("username");
                Nickname = json.Value<string>("nick_name");
                TotalCapacity = new DataSize(json.Value<long>("total"));
                UsedCapacity = new DataSize(json.Value<long>("used"));
                return true;
            }
            return false;
        }

        public Task<bool> CheckQuickAccess()
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyCollection<string>> GetDownloadUrls(string file)
        {
            return await Task.Run(() =>
            {
                var nFile = GetNetDiskFileByPath(file);
                if (nFile == null) return null;
                var json =
                    JObject.Parse(
                        new HttpClient().Post($"http://api.usmusic.cn/filelinks?token={AccUser.Token}&uk={Userid}",
                            new Dictionary<string, string>()
                            {
                                ["files"] = Uri.EscapeDataString(JArray.FromObject(new[] {nFile})
                                    .ToString(Formatting.None))
                            }));
                if (json.Value<int>("errno") != 0) return null;
                return JsonConvert.DeserializeObject<LinksFormat>(json.ToString()).Links.First().Value;
            });

        }

        public INetDiskFile GetNetDiskFileByPath(string path)
        {
            var fileName = path.Split('/').Last();
            var json = JObject.Parse(new HttpClient().Get($"http://api.usmusic.cn/filelist?token={AccUser.Token}&uk={Userid}&path={path.GetSuperPath().UrlEncode()}"));
            if (json.Value<int>("errno") != 0) return null;
            return JArray.Parse(json["list"].ToString()).Select(v =>
            {
                var file = JsonConvert.DeserializeObject<BaiduNetDiskFile>(v.ToString());
                file.User = this;
                return file;
            }).FirstOrDefault(v => v.FileName == fileName);
        }

        public Task DownloadAsync(ILazyTreeNode<INetDiskFile> fileNode, FileLocation downloadFolder, Action<ITransferTaskToken> action)
        {
            throw new NotImplementedException();
        }

        private class LinksFormat
        {
            [JsonProperty("links")]
            public Dictionary<string, string[]> Links { get; set; }
        }
    }
}

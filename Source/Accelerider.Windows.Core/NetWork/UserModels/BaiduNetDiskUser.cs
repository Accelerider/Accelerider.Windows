using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            if (fileNode.Content.FileType == FileTypeEnum.FolderType)
            {
                var redundantPathLength = fileNode.Content.FilePath.FolderPath.Length + 1;

                await fileNode.ForEachAsync(file =>
                {
                    if (file.FileType == FileTypeEnum.FolderType) return;

                    var subPath = file.FilePath.FullPath.Substring(redundantPathLength);
                    FileLocation downloadPath = Path.Combine(downloadFolder, subPath);
                    Directory.CreateDirectory(downloadPath.FolderPath);
                });


            }
            throw new NotImplementedException();
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

        public Task<IReadOnlyCollection<string>> GetDownloadUrls(string file)
        {
            throw new NotImplementedException();
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
    }
}

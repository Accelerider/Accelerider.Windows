using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Core.Files.AcceleriderCloud;
using Accelerider.Windows.Core.Tools;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Accelerider.Windows.Core.NetWork.UserModels
{
    internal sealed class AcceleriderCloudUser : NetDiskUserBase, ITaskCreator
    {
        public AcceleriderUser AccUser { get; }

        internal AcceleriderCloudUser(AcceleriderUser user)
        {
            Username = "AcceleriderCloud";
            UserId = "AcceleriderCloudUser";
            AccUser = user;
        }

        public override async Task<bool> RefreshUserInfoAsync()
        {
            return true;
        }

        public override ITransferTaskToken UploadAsync(FileLocation from, FileLocation to)
        {
            throw new NotImplementedException();
        }

        public override Task<(ShareStateCode, ISharedFile)> ShareAsync(IEnumerable<INetDiskFile> files, string password = null)
        {
            throw new NotImplementedException();
        }

        public override async Task<ILazyTreeNode<INetDiskFile>> GetNetDiskFileRootAsync()
        {
            await Task.Delay(100);
            var tree = new LazyTreeNode<INetDiskFile>(new AcceleriderCloudFile { User = AccUser })
            {
                ChildrenProvider = async parent =>
                {
                    var json =
                        JObject.Parse(await new HttpClient().GetAsync(
                            $"http://api.usmusic.cn/cloud/filelist?token={AccUser.Token}&path={parent.FilePath.FullPath.UrlEncode()}"));
                    if (json.Value<int>("errno") != 0) return null;
                    return JArray.Parse(json["list"].ToString()).Select(v =>
                    {
                        var file = JsonConvert.DeserializeObject<AcceleriderCloudFile>(v.ToString());
                        file.User = AccUser;
                        return file;
                    });
                }
            };
            return tree;
        }

        public override Task<IEnumerable<ISharedFile>> GetSharedFilesAsync()
        {
            return Task.FromResult<IEnumerable<ISharedFile>>(null);
        }

        public override Task<IEnumerable<IDeletedFile>> GetDeletedFilesAsync()
        {
            return Task.FromResult<IEnumerable<IDeletedFile>>(null);
        }

        public Task<bool> CheckQuickAccess()
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyCollection<string>> GetDownloadUrls(string file)
        {
            return await Task.Run(() =>
            {
                var json = JObject.Parse(new HttpClient().Get($"http://api.usmusic.cn/cloud/filelink?token={AccUser.Token}&path={file.UrlEncode()}"));
                return json.Value<int>("errno") != 0 ? null : JsonConvert.DeserializeObject<List<string>>(json["links"].ToString());
            });
        }

        public INetDiskFile GetNetDiskFileByPath(string path)
        {
            var fileName = path.Split('/').Last();
            var json = JObject.Parse(new HttpClient().Get($"http://api.usmusic.cn/cloud/filelist?token={AccUser.Token}&path={path.GetSuperPath().UrlEncode()}"));
            if (json.Value<int>("errno") != 0) return null;
            return JArray.Parse(json["list"].ToString()).Select(v =>
            {
                var file = JsonConvert.DeserializeObject<AcceleriderCloudFile>(v.ToString());
                file.User = AccUser;
                return file;
            }).FirstOrDefault(v => v.FileName == fileName);
        }

    }
}

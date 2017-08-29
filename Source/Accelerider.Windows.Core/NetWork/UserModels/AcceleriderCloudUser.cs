using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Core.Files.AcceleriderCloud;
using Accelerider.Windows.Core.Files.BaiduNetDisk;
using Accelerider.Windows.Core.Tools;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Accelerider.Windows.Core.NetWork.UserModels
{
    internal class AcceleriderCloudUser : INetDiskUser
    {
        public string Username => "AcceleriderCloud";
        public DataSize TotalCapacity { get; } = new DataSize(0);
        public DataSize UsedCapacity { get; } = new DataSize(0);

        public AcceleriderUser AccUser { get; }
        internal AcceleriderCloudUser(AcceleriderUser user)
        {
            AccUser = user;
        }

        public Task<bool> RefreshUserInfoAsync()
        {
            throw new NotImplementedException();
        }

        public ITransferTaskToken UploadAsync(FileLocation from, FileLocation to)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<ITransferTaskToken>> DownloadAsync(ILazyTreeNode<INetDiskFile> fileNode, FileLocation downloadFolder = null)
        {
            throw new NotImplementedException();
        }

        public Task<(ShareStateCode, ISharedFile)> ShareAsync(IEnumerable<INetDiskFile> files, string password = null)
        {
            throw new NotImplementedException();
        }

        public async Task<ILazyTreeNode<INetDiskFile>> GetNetDiskFileRootAsync()
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

        public Task<IEnumerable<ISharedFile>> GetSharedFilesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IDeletedFile>> GetDeletedFilesAsync()
        {
            throw new NotImplementedException();
        }
    }
}

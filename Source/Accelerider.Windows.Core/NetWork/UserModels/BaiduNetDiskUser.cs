using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;
using Newtonsoft.Json.Linq;

namespace Accelerider.Windows.Core.NetWork.UserModels
{
    internal class BaiduNetDiskUser : INetDiskUser
    {
        public Uri HeadImageUri { get; set; }
        public string Username { get; set; }
        public string Nickname { get; set; }
        public DataSize TotalCapacity { get; set; }
        public DataSize UsedCapacity { get; set; }

        internal string Userid { get; }

        private readonly AcceleriderUser _user;

        internal BaiduNetDiskUser(AcceleriderUser user, string userid)
        {
            _user = user;
            Userid = userid;
            var json = JObject.Parse(
                new HttpClient().Get($"http://api.usmusic.cn/userinfo?token={_user.Token}&uk={Userid}"));
            HeadImageUri = new Uri(json.Value<string>("avatar_url"));
            Username = json.Value<string>("username");
            Nickname = json.Value<string>("nick_name");
            TotalCapacity = new DataSize(json.Value<long>("total"));
            UsedCapacity = new DataSize(json.Value<long>("used"));
        }

        public ITransferTaskToken UploadAsync(FileLocation from, FileLocation to)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<ITransferTaskToken>> DownloadAsync(IAsyncTreeNode<INetDiskFile> fileNode, FileLocation downloadFolder = null)
        {
            throw new NotImplementedException();
        }

        public Task<(ShareStateCode, ISharedFile)> ShareAsync(IEnumerable<INetDiskFile> files, string password = null)
        {
            throw new NotImplementedException();
        }

        public Task<IAsyncTreeNode<INetDiskFile>> GetNetDiskFileRootAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ISharedFile>> GetSharedFilesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IDeletedFile>> GetDeletedFilesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> RefreshUserInfoAsync()
        {
            throw new NotImplementedException();
        }
    }
}

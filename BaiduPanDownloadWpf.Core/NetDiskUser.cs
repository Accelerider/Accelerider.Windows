using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaiduPanDownloadWpf.Core.NetWork;
using BaiduPanDownloadWpf.Core.NetWork.Packets;
using BaiduPanDownloadWpf.Core.ResultData;
using BaiduPanDownloadWpf.Infrastructure;
using BaiduPanDownloadWpf.Infrastructure.Interfaces;
using BaiduPanDownloadWpf.Infrastructure.Interfaces.Files;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Practices.Unity;

namespace BaiduPanDownloadWpf.Core
{
    /// <summary>
    /// 网盘账户
    /// </summary>
    public class NetDiskUser : ModelBase, INetDiskUser
    {
        private readonly MountUser _account;

        /// <summary>
        /// 便利方法,获取数据服务器
        /// </summary>
        internal Server DataServer => _account.DataServer;

        /// <summary>
        /// 便利方法,获取账户Token
        /// </summary>
        internal string Token => _account.Token;

        /// <summary>
        /// 便利方法,获取下载路径
        /// </summary>
        internal string DownloadDirectory => Container.Resolve<ILocalConfigInfo>().DownloadDirectory;

        internal MountUser LocalUser => _account;

        public NetDiskUser(IUnityContainer container, MountUser account) : base(container)
        {
            _account = account;
            RootFile = new NetDiskFile(this);
        }


        #region Public properties
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; private set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string Nickname { get; private set; }

        /// <summary>
        /// 头像URL
        /// </summary>
        public Uri HeadImageUri { get; private set; }

        /// <summary>
        /// 总容量
        /// </summary>
        public long TotalSpace { get; private set; }

        /// <summary>
        /// 剩余容量
        /// </summary>
        public long FreeSpace { get; private set; }

        /// <summary>
        /// 已用容量
        /// </summary>
        public long UsedSpace { get; private set; }

        /// <summary>
        /// Gets the root file, which represents root directory of the net-disk.
        /// </summary>
        public INetDiskFile RootFile { get; }
        #endregion


        public IEnumerable<IDiskFile> GetUncompletedFiles()
        {
            return _account.GetUncompletedFiles();
        }

        public IEnumerable<ILocalDiskFile> GetCompletedFiles()
        {
            return _account.GetCompletedFiles();
        }


        public Task DownloadAsync(NetDiskFile[] files)
        {
            throw new NotImplementedException();
        }

        public async Task<Uri> ShareFilesAsync(IEnumerable<INetDiskFile> files, string password = null)
        {
            var temp = JsonConvert.DeserializeObject<ShareResult>(await DataServer.SendPacketAsync(new ShareFilePacket
            {
                Token = Token,
                FileIds = files.Select(element=>element.FileId).ToArray(),
                Password = password
            }));
            return new Uri(temp.ShortUrl);
        }

        public Task<IEnumerable<ISharedFile>> GetSharedFilesAsync()
        {
            throw new NotImplementedException();
            //return JsonConvert.DeserializeObject<ShareListResult>(await DataServer.SendPacketAsync(new ListSharePacket
            //{
            //    Token = Token,
            //    Page = page
            //}));
        }

        public Task<IEnumerable<IDeletedFile>> GetDeletedFilesAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        public async Task UpdateAsync()
        {
            var json = JObject.Parse(await DataServer.SendPacketAsync(new PacketBase
            {
                Token = Token,
                PacketID = 10
            }));
            Username = (string)json["username"];
            Nickname = (string)json["nick_name"];
            HeadImageUri = new Uri((string)json["avatar_url"]);
            TotalSpace = (long)json["total"];
            FreeSpace = (long)json["free"];
            UsedSpace = (long)json["used"];
        }
    }
}

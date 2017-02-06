using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaiduPanDownloadWpf.Core.Download;
using BaiduPanDownloadWpf.Core.NetWork;
using BaiduPanDownloadWpf.Core.NetWork.Packets;
using BaiduPanDownloadWpf.Infrastructure;
using BaiduPanDownloadWpf.Infrastructure.Interfaces;
using BaiduPanDownloadWpf.Infrastructure.Interfaces.Files;
using Newtonsoft.Json.Linq;

namespace BaiduPanDownloadWpf.Core
{
    public class LocalDiskUser : ILocalDiskUser
    {
        #region Public properties
        /// <summary>
        /// 账号Token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 是否已绑定百度账户
        /// </summary>
        public bool BoundAccount { get; set; }

        /// <summary>
        /// 数据服务器
        /// </summary>
        public Server DataServer { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        internal string Name { get; set; }

        /// <summary>
        /// 密码 
        /// </summary>
        internal string Password { get; set; }
        #endregion

        private List<DownloadInfo> _uncompletedList=new List<DownloadInfo>();
        private List<DownloadInfo> _completedList=new List<DownloadInfo>();

        /// <summary>
        /// 绑定百度账号
        /// </summary>
        /// <param name="baiduCookies"></param>
        /// <returns></returns>
        public async Task<bool> BindingBaiduAccountAsync(Cookies baiduCookies)
        {
            if (baiduCookies == null) return false;
            var ret = await DataServer.SendPacketAsync(new BindingCookiesPacket()
            {
                Token = Token,
                cookies = baiduCookies
            });
            return ret.Contains("0");
        }

        public bool IsAutoSignIn { get; set; }

        public async Task<IEnumerable<INetDiskUser>> GetAllNetDiskUsers()
        {
            if (!BoundAccount) throw new NullReferenceException("尚未绑定百度账号");

            var user = new NetDiskUser(this);
            await user.UpdateUserInfoAsync();
            CurrentNetDiskUser = user;
            return new[] { user };
        }


        public IEnumerable<ILocalDiskFile> GetUncompletedFiles()
        {
            return _uncompletedList.Select(LocalDiskFile.GetLocalDiskFile).ToList();
        }

        public IEnumerable<ILocalDiskFile> GetCompletedFiles()
        {
            return _completedList.Select(LocalDiskFile.GetLocalDiskFile).ToList();
        }

        public void AddDownloadTask(string[] urls, string downloadPath, int thrreadNum, Cookies cookies = null)
        {

        }

        public override string ToString()
        {
            var json = new JObject
            {
                ["Name"] = Name,
                ["Pass"] = Password,
                ["AutoSignin"] = IsAutoSignIn,
                ["DownloadDirectory"] = DownloadDirectory,
                ["ParallelTaskNumber"] = ParallelTaskNumber,
                ["DwonloadTheradNumber"]=DownloadThreadNumber,
                ["RememberPassword"]=IsRememberPassword
            };
            return json.ToString();
        }

        public INetDiskUser CurrentNetDiskUser { get; set; }
        public string DownloadDirectory { get; set; } = @"D:\BaiduDownload";
        public int ParallelTaskNumber { get; set; } = 1;
        public int DownloadThreadNumber { get; set; } = 32;
        public bool IsRememberPassword { get; set; }
    }
}
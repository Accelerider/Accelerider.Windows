using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BaiduPanDownloadWpf.Core.Download;
using BaiduPanDownloadWpf.Core.NetWork;
using BaiduPanDownloadWpf.Core.NetWork.Packets;
using BaiduPanDownloadWpf.Infrastructure.Interfaces;
using BaiduPanDownloadWpf.Infrastructure.Interfaces.Files;
using Newtonsoft.Json.Linq;
using BaiduPanDownloadWpf.Core.Download.DwonloadCore;
using BaiduPanDownloadWpf.Core.ResultData;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;

namespace BaiduPanDownloadWpf.Core
{
    public class LocalDiskUser : ModelBase, ILocalDiskUser
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
            return TaskManager.GetTaskManagerByLocalDiskUser(Container, this).GetUncompletedList();
        }

        public IEnumerable<ILocalDiskFile> GetCompletedFiles()
        {
            return TaskManager.GetTaskManagerByLocalDiskUser(Container, this).GetCompletedList();
        }

        public void DownloadFile(NetDiskFile file, string path)
        {
            TaskManager.GetTaskManagerByLocalDiskUser(Container, this).CreateTask(file, path);
        }

        public TaskManager GetTaskManger()
        {
            return TaskManager.GetTaskManagerByLocalDiskUser(Container, this);
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="files"></param>
        /// <param name="method"></param>
        public async Task<DownloadResult> DownloadFiles(NetDiskFile[] files, DownloadMethod method)
        {
            return JsonConvert.DeserializeObject<DownloadResult>(await DataServer.SendPacketAsync(new DownloadPacket()
            {
                Token = Token,
                Info = files,
                Method = (int)method
            }));
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
        public string DownloadDirectory { get; set; } = Directory.GetDirectoryRoot(Directory.GetCurrentDirectory()) + "BaiduDownload";
        public int ParallelTaskNumber { get; set; } = 1;
        public int DownloadThreadNumber { get; set; } = 32;
        public bool IsRememberPassword { get; set; }
        public bool IsAutoSignIn { get; set; }

        // TODO: Temporary solution.
        public void PasueDownloadTask(long fileId)
        {
            TaskManager.GetTaskManagerByLocalDiskUser(Container, this).PauseTask(fileId);
        }

        // TODO: Temporary solution.
        public void RestartDownloadTask(long fileId)
        {
            TaskManager.GetTaskManagerByLocalDiskUser(Container, this).ContinueTask(fileId);
        }

        // TODO: Temporary solution.
        public void CancelDownloadTask(long fileId)
        {
            TaskManager.GetTaskManagerByLocalDiskUser(Container, this).RemoveTask(fileId);
        }

        public LocalDiskUser(IUnityContainer container) : base(container)
        {

        }
    }
}
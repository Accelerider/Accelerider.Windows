using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using BaiduPanDownloadWpf.Core.Download;
using BaiduPanDownloadWpf.Core.NetWork;
using BaiduPanDownloadWpf.Core.NetWork.Packets;
using BaiduPanDownloadWpf.Infrastructure.Interfaces;
using BaiduPanDownloadWpf.Infrastructure.Interfaces.Files;
using Newtonsoft.Json.Linq;
using BaiduPanDownloadWpf.Core.Download.DwonloadCore;
using BaiduPanDownloadWpf.Core.ResultData;
using BaiduPanDownloadWpf.Infrastructure;
using BaiduPanDownloadWpf.Infrastructure.Exceptions;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using Prism.Logging;
using System.Linq;

namespace BaiduPanDownloadWpf.Core
{
    public class MountUser : ModelBase, IMountUser
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
        public string Username { get; set; }

        /// <summary>
        /// 密码 
        /// </summary>
        public string PasswordEncrypted { get; set; }
        #endregion

        private List<DownloadInfo> _uncompletedList = new List<DownloadInfo>();
        private List<DownloadInfo> _completedList = new List<DownloadInfo>();
        private List<INetDiskUser> _netDiskUsers = new List<INetDiskUser>();

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

        public bool IsConnectedServer { get; private set; }

        public IEnumerable<INetDiskUser> GetAllNetDiskUsers()
        {
            if (!BoundAccount)
            {
                //MessageBox.Show("尚未绑定百度账号，请使用工具绑定");
                throw new NullReferenceException("尚未绑定百度账号，请使用工具绑定");
            }
            FillNetDiskUsers();

            //await _netDiskUsers.FirstOrDefault()?.UpdateAsync();
            return _netDiskUsers.AsReadOnly();
        }

        private void FillNetDiskUsers()
        {
            _netDiskUsers.Clear();
            _netDiskUsers.Add(new NetDiskUser(Container, this));
        }

        public void SetAccountInfo(string username, string password)
        {
            Username = username;
            PasswordEncrypted = password;
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
            if (!IsRememberPassword) return string.Empty;
            var json = new JObject
            {
                ["Username"] = Username,
                ["Password"] = PasswordEncrypted,
                ["IsAutoSignIn"] = IsAutoSignIn
            };
            return json.ToString();
        }

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
        public async void CancelDownloadTask(long fileId)
        {
            await TaskManager.GetTaskManagerByLocalDiskUser(Container, this).RemoveTask(fileId);
        }

        public async Task SignInAsync()
        {
            IsConnectedServer = false;
            DataServer = Server.TestServer;
            var json = JObject.Parse(await DataServer.SendPacketAsync(new LoginPacket()
            {
                Name = Username,
                Password = PasswordEncrypted,
            }));
            switch ((int)json["error"])
            {
                default:
                    Token = (string)json["token"];
                    BoundAccount = (bool)json["cookies"];
                    try
                    {
                        TaskManager.GetTaskManagerByLocalDiskUser(Container, this);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex.Message, Category.Exception, Priority.High);
                    }
                    IsConnectedServer = true;
                    break;
                case 1:
                    throw new LoginException("用户不存在", ClientLoginStateEnum.NonUser);
                case 2:
                    throw new LoginException("密码错误", ClientLoginStateEnum.PasswordError);
                case 3:
                    throw new LoginException("异地登录", ClientLoginStateEnum.OtherError);
                case 4:
                    throw new LoginException("服务端出现未知错误", ClientLoginStateEnum.OtherError);
                case 5:
                    throw new LoginException("账号被封禁", ClientLoginStateEnum.Baned);
            }
        }

        public void SignOut()
        {
            // Clear user information.
            IsConnectedServer = false;
            Token = null;
            _netDiskUsers = new List<INetDiskUser>();
        }

        public INetDiskUser GetCurrentNetDiskUser()
        {
            if (_netDiskUsers == null || !_netDiskUsers.Any()) FillNetDiskUsers();
            return _netDiskUsers?.FirstOrDefault();
        }

        public MountUser(IUnityContainer container) : base(container)
        {

        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accelerider.Windows.Core.DownloadEngine;
using Accelerider.Windows.Core.Files.BaiduNetDisk;
using Accelerider.Windows.Core.NetWork;
using Accelerider.Windows.Core.NetWork.UserModels;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;
using Microsoft.Practices.ObjectBuilder2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Accelerider.Windows.Core
{
    internal class AcceleriderUser : IAcceleriderUser
    {
        internal static AcceleriderUser AccUser { get; private set; }

        internal string Token { get; private set; }


        private readonly List<ITransferTaskToken> _downloadingTasks = new List<ITransferTaskToken>();
        private readonly List<ITransferTaskToken> _uploadTasks = new List<ITransferTaskToken>();

        private readonly List<ITransferedFile> _downloadedFiles = new List<ITransferedFile>();
        private readonly List<ITransferedFile> _uploadedFiles = new List<ITransferedFile>();

        public AcceleriderUser()
        {
            //InitializeNetDiskUsers();
            //CurrentNetDiskUser = NetDiskUsers[0];
            AccUser = this;
        }



        #region Accelerider account system
        public async Task<string> SignUpAsync(string username, string password, string licenseCode)
        {
            await Task.Delay(6000);
            return "Sign up failed: License code is incorrect.";
        }

        public async Task<string> SignInAsync(string username, string password)
        {
            var json = JObject.Parse(await new HttpClient().PostAsync("http://api.usmusic.cn/login?security=md5",
                new Dictionary<string, string>()
                {
                    ["name"] = username,
                    ["password"] = password,
                    ["clienttype"] = "wpf",
                    ["ver"] = "1"
                }));
            if (json.Value<int>("errno") != 0)
                return json.Value<string>("message");
            Token = json.Value<string>("token");
            await InitializeNetDiskUsers();
            var temp = DownloadTaskManager.Manager.Handles;
            return string.Empty;
        }

        public async Task<bool> SignOutAsync()
        {
            await Task.Delay(100);
            Token = string.Empty;
            return true;
        }

        #endregion

        #region Accelerider Services
        public ITransferTaskToken Upload(FileLocation from, FileLocation to)
        {
            throw new NotImplementedException();
        }

        public Task<(ShareStateCode, ISharedFile)> ShareAsync(IEnumerable<INetDiskFile> files, string password = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Operates sub-account (cloud account)
        public IReadOnlyList<INetDiskUser> NetDiskUsers { get; private set; }

        public INetDiskUser CurrentNetDiskUser { get; set; }

        public async Task<bool> AddNetDiskUserAsync(INetDiskUser user)
        {
            await Task.Delay(200);
            return true;
        }

        public Task<bool> RemoveNetDiskUserAsync(INetDiskUser user)
        {
            throw new NotImplementedException();
        }

        internal ITaskCreator GetTaskCreatorByUserid(string id)
        {
            return NetDiskUsers.FirstOrDefault(v => v.Userid == id) as ITaskCreator;
        }

        #endregion

        #region Gets Transfer tasks or files
        public IReadOnlyCollection<ITransferTaskToken> GetDownloadingTasks() => DownloadTaskManager.Manager.Handles;

        public IReadOnlyCollection<ITransferTaskToken> GetUploadingTasks() => _uploadTasks;

        public IReadOnlyCollection<ITransferedFile> GetDownloadedFiles() => _downloadedFiles;

        public IReadOnlyCollection<ITransferedFile> GetUploadedFiles() => _uploadedFiles;

        #endregion

        #region Private methods
        private async Task InitializeNetDiskUsers()
        {
            var list = new List<INetDiskUser>();
            var baidu = JObject.Parse(await new HttpClient().GetAsync("http://api.usmusic.cn/userlist?token=" + Token));
            var onedrive = JObject.Parse(await new HttpClient().GetAsync("http://api.usmusic.cn/onedrive/userlist?token=" + Token));
            if (baidu.Value<int>("errno") == 0)
                list.AddRange(baidu["userlist"].Select(v => new BaiduNetDiskUser(this, v.Value<long>("Uk").ToString())));
            list.Add(new AcceleriderCloudUser(this));
            if (onedrive.Value<int>("errno") == 0)
                list.AddRange(onedrive["userlist"].Select(v =>
                {
                    var user = JsonConvert.DeserializeObject<OneDriveUser>(v.ToString());
                    user.AccUser = this;
                    return user;
                }));
            foreach (var user in list)
                await user.RefreshUserInfoAsync();
            NetDiskUsers = list;
            CurrentNetDiskUser = NetDiskUsers[0];

        }

        #endregion
    }
}
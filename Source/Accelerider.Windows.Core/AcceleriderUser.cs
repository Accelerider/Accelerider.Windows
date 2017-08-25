using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Accelerider.Windows.Core.NetWork;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;
using Newtonsoft.Json.Linq;

namespace Accelerider.Windows.Core
{
    internal class AcceleriderUser : IAcceleriderUser
    {

        internal string Token { get; private set; }


        private readonly List<ITransferTaskToken> _downloadingTasks = new List<ITransferTaskToken>();
        private readonly List<ITransferTaskToken> _uploadTasks = new List<ITransferTaskToken>();

        private readonly List<ITransferedFile> _downloadedFiles = new List<ITransferedFile>();
        private readonly List<ITransferedFile> _uploadedFiles = new List<ITransferedFile>();

        public AcceleriderUser()
        {
            InitializeNetDiskUsers();
            //CurrentNetDiskUser = NetDiskUsers[2];
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
            return string.Empty;
        }

        public Task<bool> SignOutAsync()
        {
            throw new NotImplementedException();
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

        public Task<bool> AddNetDiskUserAsync(INetDiskUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveNetDiskUserAsync(INetDiskUser user)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Gets Transfer tasks or files
        public IReadOnlyCollection<ITransferTaskToken> GetDownloadingTasks() => _downloadingTasks;

        public IReadOnlyCollection<ITransferTaskToken> GetUploadingTasks() => _uploadTasks;

        public IReadOnlyCollection<ITransferedFile> GetDownloadedFiles() => _downloadedFiles;

        public IReadOnlyCollection<ITransferedFile> GetUploadedFiles() => _uploadedFiles;

        #endregion

        #region Private methods
        private void InitializeNetDiskUsers()
        {
            NetDiskUsers = new[]
            {
                new NetDiskUser
                {
                    HeadImageUri = new Uri("https://avatars0.githubusercontent.com/u/29689099?v=4&amp;s=100"),
                    Username = "Jielun Zhou",
                    FilePathMock = "C:\\"
                },
                new NetDiskUser
                {
                    HeadImageUri = new Uri("https://avatars3.githubusercontent.com/u/10069087?v=4&amp;s=100"),
                    Username = "Junjie Lin",
                    FilePathMock = "D:\\"
                },
                new NetDiskUser
                {
                    HeadImageUri = new Uri("https://avatars0.githubusercontent.com/u/25058219?v=4&amp;s=100"),
                    Username = "TaoFen Boy",
                    FilePathMock = "E:\\"
                },
                new NetDiskUser
                {
                    HeadImageUri = new Uri("https://avatars0.githubusercontent.com/u/26038597?v=4&amp;s=100"),
                    Username = "czy8518",
                    FilePathMock = "F:\\"
                },
                new NetDiskUser
                {
                    HeadImageUri = new Uri("https://avatars0.githubusercontent.com/u/20897838?v=4&amp;s=100"),
                    Username = "Mr-Share",
                    FilePathMock = "G:\\"
                },
                new NetDiskUser
                {
                    HeadImageUri = new Uri("https://avatars0.githubusercontent.com/u/4465021?v=4&amp;s=100"),
                    Username = "DestinyHunter",
                    FilePathMock = "H:\\"
                },
                new NetDiskUser
                {
                    HeadImageUri = new Uri("https://avatars0.githubusercontent.com/u/27673128?v=4&amp;s=100"),
                    Username = "qitiandashengsunwukong",
                    FilePathMock = "I:\\"
                },
                new NetDiskUser
                {
                    HeadImageUri = new Uri("https://avatars0.githubusercontent.com/u/9046253?v=4&amp;s=100"),
                    Username = "MegalovaniaHere",
                    FilePathMock = "E:\\"
                },
                new NetDiskUser
                {
                    HeadImageUri = new Uri("https://avatars0.githubusercontent.com/u/25217638?v=4&amp;s=100"),
                    Username = "starlightme",
                    FilePathMock = "E:\\"
                },
                new NetDiskUser
                {
                    HeadImageUri = new Uri("https://avatars0.githubusercontent.com/u/11731515?v=4&amp;s=100"),
                    Username = "Roadchen",
                    FilePathMock = "E:\\"
                },
                new NetDiskUser
                {
                    HeadImageUri = new Uri("https://avatars0.githubusercontent.com/u/4246206?v=4&amp;s=100"),
                    Username = "itodayer",
                    FilePathMock = "E:\\"
                },

            };
        }

        #endregion
    }
}
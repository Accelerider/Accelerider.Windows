using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Core
{
    internal class AcceleriderUser : IAcceleriderUser
    {
        private readonly List<ITransferTaskToken> _downloadingTasks = new List<ITransferTaskToken>();
        private readonly List<ITransferTaskToken> _uploadTasks = new List<ITransferTaskToken>();

        private readonly List<ITransferedFile> _downloadedFiles = new List<ITransferedFile>();
        private readonly List<ITransferedFile> _uploadedFiles = new List<ITransferedFile>();


        public AcceleriderUser()
        {
            InitializeNetDiskUsers();
            CurrentNetDiskUser = NetDiskUsers[2];
        }


        #region Accelerider account system
        public async Task<string> SignUpAsync(string username, string password, string licenseCode)
        {
            await Task.Delay(1000);
            return "Sign up failed: License code is incorrect.";
        }

        public async Task<string> SignInAsync(string username, string password)
        {
            await Task.Delay(2000);
            return new Random().NextDouble() > 0.8 ? "Login failed: Usename or password is incorrect." : null;
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
                    FilePathMock = "E:\\"
                },
                new NetDiskUser
                {
                    HeadImageUri = new Uri("https://avatars3.githubusercontent.com/u/10069087?v=4&amp;s=100"),
                    Username = "Junjie Lin",
                    FilePathMock = "F:\\"
                },
                new NetDiskUser
                {
                    HeadImageUri = new Uri("https://avatars0.githubusercontent.com/u/26038597?v=4&amp;s=100"),
                    Username = "TaoFen Boy",
                    FilePathMock = "G:\\"
                },
            };
        }

        #endregion
    }
}
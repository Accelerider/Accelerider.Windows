using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Core
{
    internal class AcceleriderUser : IAcceleriderUser
    {
        private readonly List<ITransferedFile> _downloadedFiles = new List<ITransferedFile>();
        private readonly List<ITransferedFile> _uploadedFiles = new List<ITransferedFile>();


        public AcceleriderUser()
        {
            InitializeNetDiskUsers();
            CurrentNetDiskUser = NetDiskUsers[2];
        }


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

        public ITransferTaskToken Upload(FileLocation filePath)
        {
            throw new NotImplementedException();
        }

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

        public IReadOnlyCollection<ITransferedFile> GetDownloadedFiles()
        {
            return _downloadedFiles;
        }

        public IReadOnlyCollection<ITransferedFile> GetUploadedFiles()
        {
            return _uploadedFiles;
        }


        private void InitializeNetDiskUsers()
        {
            NetDiskUsers = new[]
            {
                new NetDiskUser
                {
                    HeadImageUri = new Uri("https://avatars0.githubusercontent.com/u/29689099?v=4&amp;s=100"),
                    Nickname = "Jielun Zhou",
                    FilePathMock = "C:\\"
                },
                new NetDiskUser
                {
                    HeadImageUri = new Uri("https://avatars3.githubusercontent.com/u/10069087?v=4&amp;s=100"),
                    Nickname = "Junjie Lin",
                    FilePathMock = "D:\\"
                },
                new NetDiskUser
                {
                    HeadImageUri = new Uri("https://avatars0.githubusercontent.com/u/26038597?v=4&amp;s=100"),
                    Nickname = "TaoFen Boy",
                    FilePathMock = "E:\\"
                },
            };
        }
    }
}
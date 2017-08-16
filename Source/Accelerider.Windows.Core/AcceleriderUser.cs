using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Accelerider.Windows.Core.Files;
using Accelerider.Windows.Core.MockData;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Core
{
    internal class AcceleriderUser : IAcceleriderUser
    {
        public AcceleriderUser()
        {
            InitializeNetDiskUsers();
            CurrentNetDiskUser = NetDiskUsers[1];
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

        public Task<bool> LoginAsync(string username, string password)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SignOutAsync()
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<ITransferedFile> GetDownloadedFiles()
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<ITransferedFile> GetUploadedFiles()
        {
            throw new NotImplementedException();
        }


        private void InitializeNetDiskUsers()
        {
            NetDiskUsers = new[]
            {
                new NetDiskUser
                {
                    HeadImageUri = new Uri("https://avatars0.githubusercontent.com/u/29689099?v=4&amp;s=100"),
                    Nickname = "Jielun Zhou",
                    FilePathMock = "E:\\"
                },
                new NetDiskUser
                {
                    HeadImageUri = new Uri("https://avatars3.githubusercontent.com/u/10069087?v=4&amp;s=100"),
                    Nickname = "Junjie Lin",
                    FilePathMock = "F:\\"
                },
                new NetDiskUser
                {
                    HeadImageUri = new Uri("https://avatars0.githubusercontent.com/u/26038597?v=4&amp;s=100"),
                    Nickname = "TaoFen Boy",
                    FilePathMock = "G:\\"
                },
            };
        }
    }
}
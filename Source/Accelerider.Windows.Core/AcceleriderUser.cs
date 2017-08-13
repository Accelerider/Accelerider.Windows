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
            CurrentNetDiskUser = new NetDiskUser();
        }


        public IReadOnlyCollection<INetDiskUser> NetDiskUsers { get; private set; }
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
    }
}
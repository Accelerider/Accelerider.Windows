using System.Collections.Generic;
using System.Threading.Tasks;
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
            throw new System.NotImplementedException();
        }

        public Task<bool> RemoveNetDiskUserAsync(INetDiskUser user)
        {
            throw new System.NotImplementedException();
        }

        public ITransferTaskToken UploadToFilePlaza(FileLocation filePath)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> LoginAsync(string username, string password)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> SignOutAsync()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<ITransferedFile> GetDownloadedFiles()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IDiskFile> GetDownloadingFiles()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<ITransferedFile> GetUploadedFiles()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IDiskFile> GetUploadingFiles()
        {
            throw new System.NotImplementedException();
        }
    }
}
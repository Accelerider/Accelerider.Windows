using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;
using Accelerider.Windows.Modules.NetDisk.Enumerations;
using Accelerider.Windows.Modules.NetDisk.Interfaces;

namespace Accelerider.Windows.Modules.NetDisk.Models
{
    internal class NetDiskUser : INetDiskUser
    {
        public string Username { get; set; }

        public DataSize TotalCapacity { get; set; }

        public DataSize UsedCapacity { get; set; }

        public IReadOnlyCollection<FileCategory> AvailableFileCategories { get; set; }

        public Task RefreshUserInfoAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ILazyTreeNode<INetDiskFile>> GetFileRootAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IList<T>> GetFilesAsync<T>(FileCategory category) where T : IFile
        {
            throw new NotImplementedException();
        }

        public Task DownloadAsync(ILazyTreeNode<INetDiskFile> @from, FileLocator to, Action<TransferItem> callback)
        {
            throw new NotImplementedException();
        }

        public Task UploadAsync(FileLocator @from, INetDiskFile to, Action<TransferItem> callback)
        {
            throw new NotImplementedException();
        }
    }
}

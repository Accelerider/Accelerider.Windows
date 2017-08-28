using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Core.NetWork.UserModels
{
    internal class OneDriveUser : IOneDriveUser
    {
        public string Username { get; }
        public DataSize TotalCapacity { get; }
        public DataSize UsedCapacity { get; }


        internal OneDriveUser(AcceleriderUser user, string userid)
        {
            
        }

        public Task<bool> RefreshUserInfoAsync()
        {
            throw new NotImplementedException();
        }

        public ITransferTaskToken UploadAsync(FileLocation @from, FileLocation to)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<ITransferTaskToken>> DownloadAsync(ILazyTreeNode<INetDiskFile> fileNode, FileLocation downloadFolder = null)
        {
            throw new NotImplementedException();
        }

        public Task<(ShareStateCode, ISharedFile)> ShareAsync(IEnumerable<INetDiskFile> files, string password = null)
        {
            throw new NotImplementedException();
        }

        public Task<ILazyTreeNode<INetDiskFile>> GetNetDiskFileRootAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ISharedFile>> GetSharedFilesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IDeletedFile>> GetDeletedFilesAsync()
        {
            throw new NotImplementedException();
        }
    }
}

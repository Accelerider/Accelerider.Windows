using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Core.NetWork
{
    public abstract class NetDiskUserBase : INetDiskUser
    {
        public virtual string Username { get; protected set; }
        public virtual string UserId { get; protected set; }
        public virtual DataSize TotalCapacity { get; protected set; }
        public virtual DataSize UsedCapacity { get; protected set; }


        public abstract Task<bool> RefreshUserInfoAsync();
        public abstract ITransferTaskToken UploadAsync(FileLocation @from, FileLocation to);
        public abstract Task DownloadAsync(ILazyTreeNode<INetDiskFile> fileNode, FileLocation downloadFolder, Action<ITransferTaskToken> action);
        public abstract Task<(ShareStateCode, ISharedFile)> ShareAsync(IEnumerable<INetDiskFile> files, string password = null);
        public abstract Task<ILazyTreeNode<INetDiskFile>> GetNetDiskFileRootAsync();
        public abstract Task<IEnumerable<ISharedFile>> GetSharedFilesAsync();
        public abstract Task<IEnumerable<IDeletedFile>> GetDeletedFilesAsync();
    }
}

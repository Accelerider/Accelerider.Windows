using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.TransferService.WpfInteractions;
using Accelerider.Windows.Modules.NetDisk.Enumerations;
using Accelerider.Windows.Modules.NetDisk.Interfaces;

namespace Accelerider.Windows.Modules.NetDisk.Models
{
    public abstract class NetDiskUserBase : INetDiskUser
    {
        public long Id { get; set; }

        public string Username { get; set; }

        public DisplayDataSize TotalCapacity { get; set; }

        public DisplayDataSize UsedCapacity { get; set; }

        public IReadOnlyCollection<FileCategory> AvailableFileCategories { get; set; }

        public abstract Task RefreshUserInfoAsync();

	    public abstract Task<ILazyTreeNode<INetDiskFile>> GetFileRootAsync();

	    public abstract Task<IList<T>> GetFilesAsync<T>(FileCategory category) where T : IFile;

	    public abstract Task DownloadAsync(ILazyTreeNode<INetDiskFile> @from, FileLocator to, Action<TransferItem> callback);

	    public abstract Task UploadAsync(FileLocator @from, INetDiskFile to, Action<TransferItem> callback);
    }
}

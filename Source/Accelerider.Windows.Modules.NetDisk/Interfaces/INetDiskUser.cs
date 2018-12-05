using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.TransferService.WpfInteractions;
using Accelerider.Windows.Modules.NetDisk.Enumerations;
using Accelerider.Windows.Modules.NetDisk.Models;

namespace Accelerider.Windows.Modules.NetDisk.Interfaces
{
    public interface INetDiskUser
    {
        // The user info ----------------------------------------------------------------------
        long Id { get; }

        string Username { get; }

        Uri Avatar { get; }

        DisplayDataSize TotalCapacity { get; }

        DisplayDataSize UsedCapacity { get; }

        IReadOnlyCollection<FileCategory> AvailableFileCategories { get; }

        Task RefreshUserInfoAsync();

        // Gets files info from server --------------------------------------------------------

        Task<ILazyTreeNode<INetDiskFile>> GetFileRootAsync();

        Task<IList<T>> GetFilesAsync<T>(FileCategory category) where T : IFile;

        // Transfer operations ----------------------------------------------------------------

        TransferItem Download(ILazyTreeNode<INetDiskFile> @from, FileLocator to);

        // It will be throw an exception if the to.FileType is not folder type.
        Task UploadAsync(FileLocator from, INetDiskFile to, Action<TransferItem> callback);
    }
}

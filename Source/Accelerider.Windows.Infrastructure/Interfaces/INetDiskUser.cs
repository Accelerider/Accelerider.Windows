using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.Interfaces
{
    public interface INetDiskUser
    {
        // User Information ---------------------------------------------------------------
        string Username { get; }

        string Userid { get; }

        DataSize TotalCapacity { get; }

        DataSize UsedCapacity { get; }

        Task<bool> RefreshUserInfoAsync();

        // Operates net-disk file ---------------------------------------------------------
        ITransferTaskToken UploadAsync(FileLocation from, FileLocation to);

        Task<IReadOnlyCollection<ITransferTaskToken>> DownloadAsync(ILazyTreeNode<INetDiskFile> fileNode, FileLocation downloadFolder = null);

        Task<(ShareStateCode, ISharedFile)> ShareAsync(IEnumerable<INetDiskFile> files, string password = null);

        // Gets net-disk files ------------------------------------------------------------
        Task<ILazyTreeNode<INetDiskFile>> GetNetDiskFileRootAsync();

        Task<IEnumerable<ISharedFile>> GetSharedFilesAsync();

        Task<IEnumerable<IDeletedFile>> GetDeletedFilesAsync();
    }
}

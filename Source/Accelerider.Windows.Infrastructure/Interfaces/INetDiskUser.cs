using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.Interfaces
{
    public interface INetDiskUser
    {
        Uri HeadImageUri { get; }

        string Username { get; }

        string Nickname { get; }

        DataSize TotalCapacity { get; }

        DataSize UsedCapacity { get; }

        // Gets local files ---------------------------------------------------------------
        IReadOnlyCollection<ITransferTaskToken> GetDownloadingFiles();

        IReadOnlyCollection<ITransferTaskToken> GetUploadingFiles();

        // Operates net-disk file ---------------------------------------------------------
        ITransferTaskToken UploadAsync(FileLocation from, FileLocation to);

        Task<IReadOnlyCollection<ITransferTaskToken>> DownloadAsync(ITreeNodeAsync<INetDiskFile> fileNode, FileLocation downloadFolder = null);

        Task<(ShareStateCode, ISharedFile)> ShareAsync(IEnumerable<INetDiskFile> files, string password = null);

        // Gets net-disk files ------------------------------------------------------------
        Task<ITreeNodeAsync<INetDiskFile>> GetNetDiskFileRootAsync();

        Task<IEnumerable<ISharedFile>> GetSharedFilesAsync();

        Task<IEnumerable<IDeletedFile>> GetDeletedFilesAsync();
    }
}

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


        Task<ITransferTaskToken> UploadAsync(FileLocation file);

        Task<IReadOnlyCollection<ITransferTaskToken>> DownloadAsync(ITreeNodeAsync<INetDiskFile> fileNode);

        Task<(ShareStateCode, ISharedFile)> ShareFilesAsync(IEnumerable<INetDiskFile> files, string password = null);

        // Gets local files ---------------------------------------------------------------
        IReadOnlyCollection<ITransferTaskToken> GetDownloadingFiles();

        IReadOnlyCollection<ITransferTaskToken> GetUploadingFiles();

        // Gets net-disk files ------------------------------------------------------------
        Task<ITreeNodeAsync<INetDiskFile>> GetNetDiskFileTreeAsync();

        Task<IEnumerable<ISharedFile>> GetSharedFilesAsync();

        Task<IEnumerable<IDeletedFile>> GetDeletedFilesAsync();
    }
}

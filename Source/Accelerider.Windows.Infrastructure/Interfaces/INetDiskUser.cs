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


        ITransferTaskToken CreateUploadTask(FileLocation file);
        ITransferTaskToken CreateDownloadTask(INetDiskFile file);

        Task<(ShareStateCode, ISharedFile)> ShareFilesAsync(IEnumerable<INetDiskFile> files, string password = null);
        Task<bool> DeleteFileAsync(INetDiskFile file);

        #region Gets net-disk files
        Task<ITreeNodeAsync<INetDiskFile>> GetNetDiskFileTreeAsync();
        Task<IEnumerable<ISharedFile>> GetSharedFilesAsync();
        Task<IEnumerable<IDeletedFile>> GetDeletedFilesAsync();
        #endregion
    }
}

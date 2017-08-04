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

        Task<(ShareStateCode, ISharedFile)> ShareFilesAsync(IEnumerable<INetDiskFile> files, string password = null);

        ITransferTaskToken UploadFile(FileLocation filePath);

        Task<bool> DeleteFileAsync(INetDiskFile file);

        Task<ITreeNodeAsync<INetDiskFile>> GetNetDiskFileTreeAsync();

        Task<IEnumerable<IDeletedFile>> GetDeletedFilesAsync();

        Task<IEnumerable<ISharedFile>> GetSharedFilesAsync();
    }
}

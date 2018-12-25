using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;

namespace Accelerider.Windows.Modules.NetDisk.Interfaces2
{
    public interface INetDiskFileOperations
    {
        Task<ILazyTreeNode<INetDiskFile>> GetFileRootAsync();

        Task<IReadOnlyList<IDeletedFile>> GetDeletedFilesAsync();

        Task<bool> DeleteFileAsync(INetDiskFile file);

        Task<bool> RestoreFileAsync(IDeletedFile file);
    }

    public interface IDownloadOperations
    {
        IDownloadingFile Download(INetDiskFile from, FileLocator to);

        IReadOnlyList<IDownloadingFile> GetDownloadingFiles();

        IReadOnlyList<ILocalDiskFile> GetDownloadedFiles();
    }

    public interface INetDiskUser : INetDiskFileOperations, IDownloadOperations, IRefreshable
    {
        string Id { get; }

        string Username { get; }

        Uri Avatar { get; }

        (long Used, long Total) Capacity { get; }
    }
}

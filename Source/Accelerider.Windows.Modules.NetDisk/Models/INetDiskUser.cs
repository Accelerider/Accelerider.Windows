using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;

namespace Accelerider.Windows.Modules.NetDisk.Models
{
    public interface INetDiskUser : INetDiskInfo, IRefreshable, INetDiskFileOperations, IDownloadOperations, IUploadOperations
    {
    }

    public interface INetDiskInfo : INotifyPropertyChanged
    {
        string Id { get; }

        string Username { get; }

        Uri Avatar { get; }

        long UsedCapacity { get; }

        long TotalCapacity { get; }
    }

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

        void ClearDownloadFiles();
    }

    public interface IUploadOperations
    {
        // TODO
    }
}

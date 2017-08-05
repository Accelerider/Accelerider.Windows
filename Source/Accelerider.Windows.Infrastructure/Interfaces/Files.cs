using System;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.Interfaces
{
    public interface IFile
    {
        long FileId { get; }

        FileTypeEnum FileType { get; }
    }

    public interface ISharedFile : IFile
    {
        string Name { get; }
        
        DateTime SharedTime { get; }

        Uri ShareLink { get; }
        
        int DownloadedNumber { get; }

        int SavedNumber { get; }

        int VisitedNumber { get; }

        string AccessCode { get; }
    }

    public interface IDiskFile : IFile
    {
        FileLocation FilePath { get; }

        DataSize FileSize { get; }
    }

    public interface ITransferedFile : IDiskFile
    {
        DateTime CompletedTime { get; }
    }

    public interface INetDiskFile : IDiskFile
    {
        DateTime CreatedTime { get; }

        DateTime ModifiedTime { get; }

        ITransferTaskToken Download();
    }

    public interface IDeletedFile : IDiskFile
    {
        DateTime DeletedTime { get; }

        int LeftDays { get; }

        Task<bool> RestoreAsync();
    }
}

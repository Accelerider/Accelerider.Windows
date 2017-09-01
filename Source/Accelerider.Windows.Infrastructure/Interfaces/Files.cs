using System;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.Interfaces
{
    public interface IFile
    {
        FileTypeEnum FileType { get; }

        Task<bool> DeleteAsync();
    }

    public interface ISharedFile : IFile
    {
        string Title { get; }

        DateTime SharedTime { get; }

        Uri ShareLink { get; }

        string AccessCode { get; }
    }

    public interface IDiskFile : IFile
    {
        FileLocation FilePath { get; }

        DataSize FileSize { get; }
    }

    public interface ITransferedFile : IDiskFile
    {
        FileCheckStatusEnum FileCheckStatus { get; }

        DateTime CompletedTime { get; }
    }

    public interface INetDiskFile : IDiskFile
    {
        DateTime ModifiedTime { get; }
    }

    public interface IDeletedFile : IDiskFile
    {
        DateTime DeletedTime { get; }

        int LeftDays { get; }

        Task<bool> RestoreAsync();
    }
}

using System;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.Interfaces
{
    public interface IDeletable
    {
        Task<bool> DeleteAsync();
    }

    public interface IFile
    {
        FileTypeEnum FileType { get; }
    }

    public interface ISharedFile : IFile, IDeletable
    {
        string Title { get; }

        DateTime SharedTime { get; }

        Uri ShareLink { get; }

        string AccessCode { get; }
    }

    public interface IFileSummary : IFile
    {
        FileLocation FilePath { get; }

        DataSize FileSize { get; }
    }

    public interface IDiskFile : IFileSummary, IDeletable
    {
    }

    public interface ITransferedFile : IDiskFile
    {
        event EventHandler<FileCheckStatusEnum> FileChekced;

        FileCheckStatusEnum CheckStatus { get; }

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

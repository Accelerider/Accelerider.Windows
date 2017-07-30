using System;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IFile
    {
        /// <summary>
        /// 
        /// </summary>
        long FileId { get; }

        /// <summary>
        /// 
        /// </summary>
        FileTypeEnum FileType { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface ISharedFile : IFile
    {
        /// <summary>
        /// 
        /// </summary>
        DateTime SharedTime { get; }

        /// <summary>
        /// 
        /// </summary>
        Uri ShareLink { get; }

        /// <summary>
        /// 
        /// </summary>
        long DownloadedNumber { get; }

        /// <summary>
        /// 
        /// </summary>
        long SavedNumber { get; }

        /// <summary>
        /// 
        /// </summary>
        long VisitedNumber { get; }

        /// <summary>
        /// 
        /// </summary>
        string Password { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IDiskFile : IFile
    {
        /// <summary>
        /// 
        /// </summary>
        FileLocation FilePath { get; }

        /// <summary>
        /// 
        /// </summary>
        DataSize FileSize { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface ITransferedFile : IDiskFile
    {
        /// <summary>
        /// 
        /// </summary>
        DateTime CompletedTime { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface INetDiskFile : IDiskFile
    {
        /// <summary>
        /// 
        /// </summary>
        DateTime CreatedTime { get; }

        /// <summary>
        /// 
        /// </summary>
        DateTime ModifiedTime { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ITransferTaskToken Download();
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IDeletedFile : IDiskFile
    {
        /// <summary>
        /// 
        /// </summary>
        DateTime DeletedTime { get; }

        /// <summary>
        /// 
        /// </summary>
        int LeftDays { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<bool> RestoreAsync();
    }
}

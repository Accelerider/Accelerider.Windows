using System;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Modules.NetDisk.Enumerations;
using Accelerider.Windows.TransferService.WpfInteractions;

namespace Accelerider.Windows.Modules.NetDisk.Interfaces
{
    public interface IDeletable
    {
        Task<bool> DeleteAsync();
    }

    public interface IFile
    {
        FileType Type { get; }
    }

    public interface ISharedFile : IFile, IDeletable
    {
        string Title { get; }

        DateTime SharedTime { get; }

        Uri AccessLink { get; }

        string AccessCode { get; }
    }

    public interface IFileSummary : IFile
    {
        FileLocator Path { get; }

        DisplayDataSize Size { get; }
    }

    public interface IDiskFile : IFileSummary, IDeletable
    {
    }

    public interface ILocalDiskFile : IDiskFile
    {
        bool Exists { get; }

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

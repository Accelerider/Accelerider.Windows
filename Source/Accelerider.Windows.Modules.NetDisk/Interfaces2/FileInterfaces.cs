using System;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Modules.NetDisk.Enumerations;
using Accelerider.Windows.TransferService;

namespace Accelerider.Windows.Modules.NetDisk.Interfaces2
{
    public interface IFileSummary
    {
        FileType Type { get; }

        FileLocator Path { get; }

        long Size { get; }
    }

    public interface INetDiskFile : IFileSummary
    {
        DateTime ModifiedTime { get; }
    }

    public interface IDownloadingFile
    {
        INetDiskUser Owner { get; }

        INetDiskFile File { get; }

        IDownloader Downloader { get; }
    }

    public interface ILocalDiskFile : IFileSummary
    {
        bool Exists { get; }

        DateTime CompletedTime { get; }
    }

    public interface IDeletedFile : IFileSummary
    {
        DateTime DeletedTime { get; }

        int LeftDays { get; }
    }
}

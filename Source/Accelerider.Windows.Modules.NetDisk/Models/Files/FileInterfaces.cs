using System;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Modules.NetDisk.Enumerations;
using Accelerider.Windows.TransferService;
using Accelerider.Windows.TransferService.WpfInteractions;

namespace Accelerider.Windows.Modules.NetDisk.Models
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

        ITransferInfo<DownloadContext> DownloadInfo { get; }

        BindableDownloader BindableDownloader { get; }

        IManagedTransporterToken Operations { get; }
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

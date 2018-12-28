using System.Windows;
using Accelerider.Windows.TransferService;
using Accelerider.Windows.TransferService.WpfInteractions;

namespace Accelerider.Windows.Modules.NetDisk.Models
{
    internal class DownloadingFile : IDownloadingFile
    {
        public INetDiskUser Owner { get; }

        public INetDiskFile File { get; }

        public ITransferInfo<DownloadContext> DownloadInfo { get; }

        public BindableDownloader BindableDownloader { get; }

        public IManagedTransporterToken Operations { get; }

        private DownloadingFile(INetDiskUser owner, INetDiskFile file, IDownloader downloader)
        {
            Owner = owner;
            File = file;
            DownloadInfo = downloader;
            BindableDownloader = downloader.ToBindable(Application.Current.Dispatcher);
            Operations = downloader.AsManaged("net-disk"); // TODO: To const.
        }

        public static IDownloadingFile Create(INetDiskUser owner, INetDiskFile file, IDownloader downloader)
        {
            return new DownloadingFile(owner, file, downloader);
        }
    }
}

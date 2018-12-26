using System;
using Accelerider.Windows.Modules.NetDisk.Models;
using Accelerider.Windows.TransferService;

namespace Accelerider.Windows.Modules.NetDisk
{
    public static class NetDiskFileExtensions
    {
        private class DownloadingFileImpl : IDownloadingFile
        {
            public INetDiskUser Owner { get; set; }

            public INetDiskFile File { get; set; }

            public IDownloader Downloader { get; set; }
        }

        public static IDownloadingFile ToDownloadingFile(this INetDiskFile @this, INetDiskUser owner, Action<IDownloaderBuilder> builderConfigure)
        {
            var builder = FileTransferService.GetDownloaderBuilder();
            builderConfigure(builder);

            return new DownloadingFileImpl
            {
                Owner = owner,
                File = @this,
                Downloader = builder.Build()
            };
        }
    }
}

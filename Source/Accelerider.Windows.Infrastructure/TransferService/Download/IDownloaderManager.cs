using System;
using System.Collections.Generic;


namespace Accelerider.Windows.Infrastructure.TransferService
{
    public interface IDownloaderManager : IJsonable<IDownloaderManager>
    {
        IEnumerable<IDownloader> Downloaders { get; }

        int MaxConcurrent { get; set; }

        bool Add(IDownloader downloader);

        void AsNext(Guid id);

        void Ready(Guid id);

        void StartAll();

        void SuspendAll();
    }
}

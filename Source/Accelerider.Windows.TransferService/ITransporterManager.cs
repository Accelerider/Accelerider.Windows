using System;
using System.Collections.Generic;

namespace Accelerider.Windows.TransferService
{
    public interface ITransporterManager<TTransferInfo, TContext>
        where TTransferInfo : ITransferInfo<TContext>
    {
        IEnumerable<TTransferInfo> Transporters { get; }

        int MaxConcurrent { get; set; }

        bool Add(TTransferInfo transporter);

        void AsNext(Guid id);

        void Ready(Guid id);

        void Suspend(Guid id);

        void StartAll();

        void SuspendAll();
    }

    public interface IDownloaderManager : ITransporterManager<IDownloader, DownloadContext>
    {
    }
}

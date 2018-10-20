using System;
using System.Collections.Generic;
using Accelerider.Windows.Infrastructure;

namespace Accelerider.Windows.TransferService
{
    public interface ITransporterManager<TTransporter, TContext>
        where TContext : TransferContextBase
        where TTransporter : ITransporter<TContext>
    {
        IEnumerable<TTransporter> Transporters { get; }

        int MaxConcurrent { get; set; }

        bool Add(TTransporter transporter);

        void AsNext(Guid id);

        void Ready(Guid id);

        void Suspend(Guid id);

        void StartAll();

        void SuspendAll();
    }

    public interface IDownloaderManager : ITransporterManager<IDownloader, DownloadContext>, IJsonable<IDownloaderManager>
    {
    }
}

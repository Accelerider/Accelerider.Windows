using System;
using System.Collections.Generic;


namespace Accelerider.Windows.Infrastructure.TransferService
{
    public interface ITransporterManager<T> : IJsonable<ITransporterManager<T>> where T : ITransporter
    {
        IEnumerable<T> Transporters { get; }

        int MaxConcurrent { get; set; }

        bool Add(T downloader);

        void AsNext(Guid id);

        void Ready(Guid id);

        void Suspend(Guid id);

        void StartAll();

        void SuspendAll();
    }
}

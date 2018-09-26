using System;
using System.ComponentModel;
using Accelerider.Windows.Infrastructure.FileTransferService;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    public interface IDownloader : IObservable<BlockTransferContext>, IDisposable, INotifyPropertyChanged
    {
        TransferStatus Status { get; }

        TransferContext Context { get; }

        void Activate();

        void Suspend();
    }
}

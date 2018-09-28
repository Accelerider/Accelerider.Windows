using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.FileTransferService;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    internal class FileDownloader : ObservableBase<BlockTransferContext>, IDownloader
    {
        private readonly Func<CancellationToken, Task<IObservable<BlockTransferContext>>> _observableFactory;
        private readonly ObserverList<BlockTransferContext> _observerList = new ObserverList<BlockTransferContext>();

        private IDisposable _disposable;
        private TransferStatus _status;
        private TransferContext _context;

        public TransferStatus Status
        {
            get => _status;
            private set => SetProperty(ref _status, value);
        }

        public TransferContext Context
        {
            get => _context;
            internal set => SetProperty(ref _context, value);
        }

        public FileDownloader(Func<CancellationToken, Task<IObservable<BlockTransferContext>>> observableFactory)
        {
            _observableFactory = observableFactory ?? throw new ArgumentNullException(nameof(observableFactory));
        }

        protected override IDisposable SubscribeCore(IObserver<BlockTransferContext> observer)
        {
            ThrowIfDisposed();

            _observerList.Add(observer);

            return Disposable.Create(() => _observerList.Remove(observer));
        }

        public async Task ActivateAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            switch (Status)
            {
                case TransferStatus.Ready: // [Start]
                    _disposable = (await _observableFactory(cancellationToken)).Subscribe(_observerList);
                    break;
                case TransferStatus.Suspended: // [Restart]
                    break;
                case TransferStatus.Faulted: // [Retry]
                    Dispose(true);
                    _disposable = (await _observableFactory(cancellationToken)).Subscribe(_observerList);
                    break;
                default:
                    return;
            }

            Status = TransferStatus.Transferring;
        }

        public void Suspend()
        {
            ThrowIfDisposed();
            if (Status != TransferStatus.Transferring) return;

            // 1. Dispose this instance to stop downloding.
            Dispose(true);
            // 2. Persist block transfer context.


            Status = TransferStatus.Suspended;
        }

        #region Implements IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (Status == TransferStatus.Disposed) return;

            if (disposing)
            {
                _disposable?.Dispose();
                _disposable = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);

            Status = TransferStatus.Disposed;
        }

        private void ThrowIfDisposed()
        {
            if (Status == TransferStatus.Disposed)
                throw new ObjectDisposedException(
                    $"{nameof(FileDownloader)}: {Context.Id:B}",
                    "This transfer task has been disposed, please re-create a task by FileTransferService if it needs to be re-downloaded.");
        }

        #endregion

        #region Implements INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;
            storage = value;
            RaisePropertyChanged(propertyName);
            return true;
        }

        #endregion
    }
}

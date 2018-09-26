using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using Accelerider.Windows.Infrastructure.FileTransferService;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    internal class DefaultFileDownloader : ObservableBase<BlockTransferContext>, ITransporter
    {
        private readonly IConnectableObservable<BlockTransferContext> _connectableObservable;

        private IDisposable _connectableObservableDisposable;
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

        public DefaultFileDownloader(IConnectableObservable<BlockTransferContext> connectableObservable)
        {
            _connectableObservable = connectableObservable ?? throw new ArgumentNullException(nameof(connectableObservable));
        }

        protected override IDisposable SubscribeCore(IObserver<BlockTransferContext> observer)
        {
            ThrowIfDisposed();

            return _connectableObservable.Subscribe(
                observer.OnNext,
                error =>
                {
                    Status = TransferStatus.Faulted;
                    observer.OnError(error);
                },
                () =>
                {
                    Status = TransferStatus.Completed;
                    observer.OnCompleted();
                });
        }

        public void Activate()
        {
            ThrowIfDisposed();

            switch (Status)
            {
                case TransferStatus.Ready: // [Start]
                    if (_connectableObservableDisposable == null)
                        _connectableObservableDisposable = _connectableObservable.Connect();
                    break;
                case TransferStatus.Suspended: // [Restart]
                    break;
                case TransferStatus.Faulted: // [Retry]
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
                _connectableObservableDisposable?.Dispose();
                _connectableObservableDisposable = null;
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
                    $"{nameof(DefaultFileDownloader)}: {Context.Id:B}",
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

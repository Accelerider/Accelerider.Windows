using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    internal class FileDownloader : ObservableBase<BlockTransferContext>, IDownloader
    {
        private readonly ObserverList<BlockTransferContext> _observerList = new ObserverList<BlockTransferContext>();
        private readonly Func<CancellationToken, Task<IEnumerable<BlockTransferContext>>> _blockTransferContextGenerator;
        private readonly Func<BlockTransferContext, IObservable<BlockTransferContext>> _blockDownloadItemFactory;
        private readonly TransferSettings _settings;

        private ConcurrentDictionary<Guid, BlockTransferContext> _blockTransferContextCache;
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

        public FileDownloader(
            Func<CancellationToken, Task<IEnumerable<BlockTransferContext>>> blockTransferContextGenerator,
            Func<BlockTransferContext, IObservable<BlockTransferContext>> blockDownloadItemFactory,
            TransferContext context,
            TransferSettings settings)
        {
            Context = context;
            _settings = settings;
            _blockTransferContextGenerator = blockTransferContextGenerator;
            _blockDownloadItemFactory = blockDownloadItemFactory;
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
                    await _settings.BuildPolicy.ExecuteAsync(async () => _disposable = await Start(cancellationToken));
                    break;
                case TransferStatus.Suspended: // [Restart]
                    _disposable = Resume();
                    break;
                case TransferStatus.Faulted: // [Retry]
                    Dispose(true);
                    await _settings.BuildPolicy.ExecuteAsync(async () => _disposable = await Start(cancellationToken));
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

            Dispose(true);

            Status = TransferStatus.Suspended;
        }

        private async Task<IDisposable> Start(CancellationToken cancellationToken)
        {
            var blockContexts = (await _blockTransferContextGenerator(cancellationToken)).ToArray();

            _blockTransferContextCache = new ConcurrentDictionary<Guid, BlockTransferContext>(
                blockContexts.ToDictionary(item => item.Id));

            return CreateAndRunBlockDownloadItems(blockContexts);
        }

        private IDisposable Resume()
        {
            var blockContexts = _blockTransferContextCache.Values;

            return CreateAndRunBlockDownloadItems(blockContexts);
        }

        private IDisposable CreateAndRunBlockDownloadItems(IEnumerable<BlockTransferContext> blockContexts)
        {
            return blockContexts
                .Select(item => _blockDownloadItemFactory(item))
                .Merge(_settings.MaxConcurrent)
                .Do(context => _blockTransferContextCache[context.Id] = context)
                .Subscribe(_observerList);
        }

        #region Implements IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (Status == TransferStatus.Disposed) return;

            if (disposing)
            {
                _disposable?.Dispose();
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

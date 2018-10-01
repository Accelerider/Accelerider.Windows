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
    internal class FileDownloader : ObservableBase<(Guid Id, int Bytes)>, IDownloader
    {
        internal class Builders
        {
            public Func<DownloadContext, Func<CancellationToken, Task<IEnumerable<BlockTransferContext>>>> BlockTransferContextGeneratorBuilder { get; set; }

            public Func<TransferSettings, Func<BlockTransferContext, IObservable<(Guid Id, int Bytes)>>> BlockDownloadItemFactoryBuilder { get; set; }

            public Func<HashSet<string>, IRemotePathProvider> RemotePathProviderBuilder { get; set; }

            public Func<string, string> LocalPathInterceptor { get; set; }

            public Func<DownloadContext, TransferSettings> TransferSettingsBuilder { get; set; }
        }


        private readonly ObserverList<(Guid Id, int Bytes)> _observerList = new ObserverList<(Guid Id, int Bytes)>();
        private readonly Builders _builders;

        private readonly HashSet<string> _remotePaths = new HashSet<string>();
        private string _localPath;

        private TransferSettings _settings;
        private ConcurrentDictionary<Guid, BlockTransferContext> _blockTransferContextCache;
        private IDisposable _disposable;

        private TransferStatus _status;
        private DownloadContext _context;

        public TransferStatus Status
        {
            get => _status;
            private set => SetProperty(ref _status, value);
        }

        public DownloadContext Context
        {
            get => _context;
            private set
            {
                if (SetProperty(ref _context, value))
                {
                    _settings = value != null ? _builders.TransferSettingsBuilder(value) : null;
                }
            }
        }

        public IReadOnlyDictionary<Guid, BlockTransferContext> BlockContexts => _blockTransferContextCache;

        public FileDownloader(Builders builders)
        {
            _builders = builders;
        }

        public IDownloader From(string path)
        {
            Guards.ThrowIfNullReference(path);

            if (_remotePaths.Add(path))
            {
                Context = null;
            }

            return this;
        }

        public IDownloader From(IEnumerable<string> paths)
        {
            Guards.ThrowIfNullReference(paths);

            _remotePaths.UnionWith(paths);
            Context = null;
            return this;
        }

        public IDownloader To(string path)
        {
            Guards.ThrowIfNullReference(path);

            if (!path.Equals(_localPath, StringComparison.InvariantCultureIgnoreCase))
            {
                _localPath = path;
                Context = null;
            }

            return this;
        }


        protected override IDisposable SubscribeCore(IObserver<(Guid Id, int Bytes)> observer)
        {
            ThrowIfDisposed();

            _observerList.Add(observer);

            return Disposable.Create(() => _observerList.Remove(observer));
        }

        public async Task ActivateAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            InitializeContext();

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

        private void InitializeContext()
        {
            if (Context == null && !string.IsNullOrEmpty(_localPath) && _remotePaths.Any(item => !string.IsNullOrEmpty(item)))
            {
                Context = new DownloadContext
                {
                    RemotePathProvider = _builders.RemotePathProviderBuilder(_remotePaths),
                    LocalPath = _builders.LocalPathInterceptor(_localPath)
                };
            }
        }

        public string ToJson()
        {
            ThrowIfTransferring();

            return (Context, _blockTransferContextCache.Values.ToList()).ToJson();
        }

        public IDownloader FromJson(string json)
        {
            ThrowIfTransferring();

            Status = TransferStatus.Ready;
            var (context, blockContexts) = json.ToObject<(DownloadContext, List<BlockTransferContext>)>();
            Context = context;
            _blockTransferContextCache = new ConcurrentDictionary<Guid, BlockTransferContext>(
                blockContexts.ToDictionary(item => item.Id));
            Status = TransferStatus.Suspended;

            return this;
        }

        private async Task<IDisposable> Start(CancellationToken cancellationToken)
        {
            var blockContexts = (await _builders.BlockTransferContextGeneratorBuilder(Context).Invoke(cancellationToken)).ToArray();

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
                .Select(item => _builders.BlockDownloadItemFactoryBuilder(_settings).Invoke(item))
                .Merge(_settings.MaxConcurrent)
                .Do(notification => _blockTransferContextCache[notification.Id].CompletedSize += notification.Bytes)
                .Subscribe(
                    value => _observerList.OnNext(value),
                    error =>
                    {
                        Status = TransferStatus.Faulted;
                        _observerList.OnError(error);
                    },
                    () =>
                    {
                        Status = TransferStatus.Completed;
                        _observerList.OnCompleted();
                    });
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

        private void ThrowIfTransferring()
        {
            if (Status == TransferStatus.Transferring)
                throw new InvalidOperationException("This operation cannot be executed during transferring. ");
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

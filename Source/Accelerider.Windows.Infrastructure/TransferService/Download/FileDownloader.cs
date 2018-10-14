using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace Accelerider.Windows.Infrastructure.TransferService
{
    internal class FileDownloader : ObservableBase<TransferNotification>, IDownloader
    {
        internal class Builders
        {
            public Func<DownloadContext, Func<CancellationToken, Task<IEnumerable<BlockTransferContext>>>> BlockTransferContextGeneratorBuilder { get; set; }

            public Func<DownloadSettings, Func<BlockTransferContext, IObservable<(Guid Id, int Bytes)>>> BlockDownloadItemFactoryBuilder { get; set; }

            public Func<HashSet<string>, IRemotePathProvider> RemotePathProviderBuilder { get; set; }

            public Func<string, string> LocalPathInterceptor { get; set; }

            public Func<DownloadContext, DownloadSettings> TransferSettingsBuilder { get; set; }
        }

        private class SerializedData
        {
            [JsonProperty]
            public object Tag { get; internal set; }

            [JsonProperty]
            public DownloadContext Context { get; internal set; }

            [JsonProperty]
            public List<BlockTransferContext> BlockContexts { get; internal set; }
        }

        private readonly ObserverList<TransferNotification> _observerList = new ObserverList<TransferNotification>();
        private readonly Builders _builders;

        private readonly HashSet<string> _remotePaths = new HashSet<string>();
        private string _localPath;
        private DownloadSettings _settings;
        private ConcurrentDictionary<Guid, BlockTransferContext> _blockTransferContextCache;
        private IDisposable _disposable;
        private TransferStatus _status;
        private DownloadContext _context;

        public Guid Id { get; } = Guid.NewGuid();

        public TransferStatus Status
        {
            get => _status;
            private set { if (SetProperty(ref _status, value)) _observerList.OnNext(new TransferNotification(Guid.Empty, value, 0)); }
        }

        public DownloadContext Context
        {
            get => _context;
            private set { if (SetProperty(ref _context, value)) _settings = value != null ? _builders.TransferSettingsBuilder(value) : null; }
        }

        public IReadOnlyDictionary<Guid, BlockTransferContext> BlockContexts => _blockTransferContextCache;

        public object Tag { get; set; }


        public FileDownloader(Builders builders)
        {
            _builders = builders;
        }


        public IDownloader From(string path)
        {
            ThrowIfTransferring();
            Guards.ThrowIfNullOrEmpty(path);

            if (_remotePaths.Add(path))
            {
                Reset();
            }

            return this;
        }

        public IDownloader From(IEnumerable<string> paths)
        {
            ThrowIfTransferring();
            Guards.ThrowIfNull(paths);
            var pathArray = paths.ToArray();
            Guards.ThrowIfNullOrEmpty(pathArray);

            _remotePaths.UnionWith(pathArray);
            Reset();
            return this;
        }

        public IDownloader To(string path)
        {
            ThrowIfTransferring();
            Guards.ThrowIfNullOrEmpty(path);

            if (!path.Equals(_localPath, StringComparison.InvariantCultureIgnoreCase))
            {
                _localPath = path;
                Reset();
            }

            return this;
        }

        public string ToJson()
        {
            ThrowIfTransferring();

            return new SerializedData
            {
                Tag = Tag,
                Context = Context,
                BlockContexts = _blockTransferContextCache.Values.ToList()
            }.ToJson();
        }

        public IDownloader FromJson(string json)
        {
            ThrowIfTransferring();
            Guards.ThrowIfNull(json);

            var serializedData = json.ToObject<SerializedData>();

            if (serializedData?.Context != null && serializedData.BlockContexts != null)
            {
                Tag = serializedData.Tag;

                Context = serializedData.Context;

                serializedData.BlockContexts.ForEach(item => item.LocalPath = Context.LocalPath);
                _blockTransferContextCache = new ConcurrentDictionary<Guid, BlockTransferContext>(
                    serializedData.BlockContexts.ToDictionary(item => item.Id));

                Status = TransferStatus.Suspended;
            }

            return this;
        }

        public async Task ActivateAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            try
            {
                await ActivateAsyncInternal(cancellationToken);
            }
            catch (Exception e)
            {
                _status = TransferStatus.Faulted;
                _observerList.OnError(e);
            }
        }

        public void Suspend()
        {
            ThrowIfDisposed();
            if (Status != TransferStatus.Transferring) return;

            Dispose(true);

            Status = TransferStatus.Suspended;
        }

        protected override IDisposable SubscribeCore(IObserver<TransferNotification> observer)
        {
            ThrowIfDisposed();

            _observerList.Add(observer);

            return Disposable.Create(() => _observerList.Remove(observer));
        }


        private async Task ActivateAsyncInternal(CancellationToken cancellationToken = default)
        {
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
                    Status = TransferStatus.Ready;
                    await ActivateAsync(cancellationToken);
                    break;
                default:
                    return;
            }

            Status = TransferStatus.Transferring;
        }

        private void InitializeContext()
        {
            if (Status == TransferStatus.Ready)
            {
                Context = new DownloadContext(Id)
                {
                    RemotePathProvider = _builders.RemotePathProviderBuilder(_remotePaths),
                    LocalPath = _builders.LocalPathInterceptor(_localPath)
                };
            }
        }

        private void Reset()
        {
            Status = TransferStatus.Ready;
            Context = null;
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
                .Do(item => _blockTransferContextCache[item.Id].CompletedSize += item.Bytes)
                .Select(item => new TransferNotification(item.Id, Status, item.Bytes))
                .Subscribe(
                    value => _observerList.OnNext(value),
                    error =>
                    {
                        _status = TransferStatus.Faulted;
                        _observerList.OnError(error);
                    },
                    () =>
                    {
                        _status = TransferStatus.Completed;
                        _observerList.OnCompleted();
                    });
        }

        private static bool SetProperty<T>(ref T storage, T value)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;
            storage = value;
            return true;
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
                    $"{nameof(FileDownloader)}: {Id:B}",
                    "This transfer task has been disposed, please re-create a task by FileTransferService if it needs to be re-downloaded.");
        }

        private void ThrowIfTransferring()
        {
            if (Status == TransferStatus.Transferring)
                throw new InvalidOperationException("This operation cannot be executed during transferring. ");
        }

        #endregion
    }
}

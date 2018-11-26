using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;

namespace Accelerider.Windows.TransferService
{
    internal class FileDownloader : ObservableBase<TransferNotification>, IDownloader
    {
        internal class BuildInfo
        {
            public DownloadContext Context { get; set; }

            public DownloadSettings Settings { get; set; }

            public Func<DownloadContext, Func<CancellationToken, Task<IEnumerable<BlockTransferContext>>>> BlockTransferContextGeneratorBuilder { get; set; }

            public Func<DownloadSettings, Func<BlockTransferContext, IObservable<(long Offset, int Bytes)>>> BlockDownloadItemFactoryBuilder { get; set; }
        }

        private readonly ObserverList<TransferNotification> _observerList = new ObserverList<TransferNotification>();
        private readonly BuildInfo _buildInfo;

        private readonly AsyncLocker _runAsyncLocker = new AsyncLocker();
        private ConcurrentDictionary<long, BlockTransferContext> _blockTransferContextCache;
        private IDisposable _disposable;
        private TransferStatus _status;
        private CancellationTokenSource _cancellationTokenSource;

        public Guid Id => Context.Id;

        public TransferStatus Status
        {
            get => _status;
            private set { if (SetProperty(ref _status, value)) _observerList.OnNext(new TransferNotification(BlockTransferContext.InvalidOffset, value, 0)); }
        }

        public DownloadContext Context => _buildInfo.Context;
        //{
        //    get => _context;
        //    private set { if (SetProperty(ref _context, value)) _settings = value != null ? _buildInfo.TransferSettingsBuilder(value) : null; }
        //}

        public IReadOnlyDictionary<long, BlockTransferContext> BlockContexts => _blockTransferContextCache;

        public object Tag { get; set; }


        public FileDownloader(BuildInfo builders)
        {
            _buildInfo = builders;
        }

        public void Run()
        {
            ThrowIfDisposed();

            _runAsyncLocker.Await(RunAsync, executeAfterUnlocked: false);
        }

        public void Stop()
        {
            ThrowIfDisposed();

            switch (Status)
            {
                case TransferStatus.Ready:
                    _cancellationTokenSource?.Cancel();
                    return;
                case TransferStatus.Transferring:
                    Dispose(true);
                    Status = TransferStatus.Suspended;
                    break;
            }
        }

        protected override IDisposable SubscribeCore(IObserver<TransferNotification> observer)
        {
            ThrowIfDisposed();

            _observerList.Add(observer);

            return Disposable.Create(() => _observerList.Remove(observer));
        }

        private async Task RunAsync()
        {
            try
            {
                if (_cancellationTokenSource == null || _cancellationTokenSource.IsCancellationRequested)
                    _cancellationTokenSource = new CancellationTokenSource();

                await ActivateAsync(_cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
            }
            catch (WebException e) when (e.Status == WebExceptionStatus.RequestCanceled)
            {
            }
            catch (Exception e)
            {
                _status = TransferStatus.Faulted;
                _observerList.OnError(e);
            }
        }

        private async Task ActivateAsync(CancellationToken cancellationToken = default)
        {
            switch (Status)
            {
                case TransferStatus.Ready: // [Start]
                    await _buildInfo.Settings.BuildPolicy.ExecuteAsync(async () => _disposable = await Start(cancellationToken));
                    break;
                case TransferStatus.Suspended: // [Restart]
                    _disposable = Resume();
                    break;
                case TransferStatus.Faulted: // [Retry]
                    Dispose(true);
                    await ActivateAsync(cancellationToken);
                    break;
            }
        }

        private async Task<IDisposable> Start(CancellationToken cancellationToken)
        {
            var blockContexts = (await _buildInfo.BlockTransferContextGeneratorBuilder(Context).Invoke(cancellationToken)).ToArray();

            _blockTransferContextCache = new ConcurrentDictionary<long, BlockTransferContext>(
                blockContexts.ToDictionary(item => item.Offset));

            return CreateAndRunBlockDownloadItems(blockContexts);
        }

        private IDisposable Resume()
        {
            var blockContexts = _blockTransferContextCache.Values;

            return CreateAndRunBlockDownloadItems(blockContexts);
        }

        private IDisposable CreateAndRunBlockDownloadItems(IEnumerable<BlockTransferContext> blockContexts)
        {
            var disposable = blockContexts
                .Select(item => _buildInfo.BlockDownloadItemFactoryBuilder(_buildInfo.Settings).Invoke(item))
                .Merge(_buildInfo.Settings.MaxConcurrent)
                .Do(item => _blockTransferContextCache[item.Offset].CompletedSize += item.Bytes)
                .Select(item => new TransferNotification(item.Offset, Status, item.Bytes))
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

            Status = TransferStatus.Transferring;

            return disposable;
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
                _cancellationTokenSource?.Cancel();
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

        #endregion
    }
}

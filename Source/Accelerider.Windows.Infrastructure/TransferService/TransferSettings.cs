using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Polly;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    public class TransferSettings
    {
        internal TransferSettings() { }

        public IAsyncPolicy<IEnumerable<IObservable<BlockTransferContext>>> BuildPolicy { get; set; }

        public BlockDownloadItemExceptionHandlers Handlers { get; internal set; }

        public int MaxConcurrent { get; set; }
    }

    public enum HandleCommand
    {
        Throw,
        Retry,
        Break
    }

    public delegate HandleCommand BlockDownloadItemExceptionHandler<in TException>(TException exception, int retryCount, BlockTransferContext context) where TException : Exception;

    public class BlockDownloadItemExceptionHandlers
    {
        private readonly ConcurrentDictionary<Guid, int> _retryStatistics = new ConcurrentDictionary<Guid, int>();
        private readonly ConcurrentDictionary<Type, BlockDownloadItemExceptionHandler<Exception>> _handlers = new ConcurrentDictionary<Type, BlockDownloadItemExceptionHandler<Exception>>();
        private readonly Func<BlockTransferContext, IObservable<BlockTransferContext>> _blockDownloadItemFactory;

        internal BlockDownloadItemExceptionHandlers(Func<BlockTransferContext, IObservable<BlockTransferContext>> blockDownloadItemFactory)
        {
            _blockDownloadItemFactory = blockDownloadItemFactory ?? throw new ArgumentNullException(nameof(blockDownloadItemFactory));
        }

        public BlockDownloadItemExceptionHandlers Catch<TException>(BlockDownloadItemExceptionHandler<TException> handler)
            where TException : Exception
        {
            var type = typeof(TException);
            if (_handlers.ContainsKey(type))
                throw new ArgumentException($"A handler that handles {typeof(TException)} exceptions already exists. ");

            _handlers[type] = (e, rertyCount, context) => handler((TException)e, rertyCount, context);
            return this;
        }

        public Func<IObservable<BlockTransferContext>, IObservable<BlockTransferContext>> ToInterceptor()
        {
            return observable => observable.Catch<BlockTransferContext, BlockTransferException>(e =>
            {
                var handler = _handlers.FirstOrDefault(item => item.Key.IsInstanceOfType(e.InnerException)).Value;
                if (handler != null)
                {
                    switch (handler(e.InnerException, GetRetryCount(e.Context.Id), e.Context))
                    {
                        case HandleCommand.Throw:
                            return Observable.Throw<BlockTransferContext>(e.InnerException ?? e);
                        case HandleCommand.Retry:
                            SetRetryCount(e.Context.Id);
                            return _blockDownloadItemFactory(e.Context);
                        case HandleCommand.Break:
                            return Observable.Empty<BlockTransferContext>();
                    }
                }

                return Observable.Throw<BlockTransferContext>(e.InnerException ?? e);
            });
        }

        public int GetRetryCount(Guid id) => _retryStatistics.ContainsKey(id) ? _retryStatistics[id] : 0;

        public void SetRetryCount(Guid id)
        {
            if (!_retryStatistics.ContainsKey(id))
            {
                _retryStatistics[id] = 0;
            }
            _retryStatistics[id]++;
        }
    }
}

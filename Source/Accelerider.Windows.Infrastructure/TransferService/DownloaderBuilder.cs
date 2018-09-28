using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using static Accelerider.Windows.Infrastructure.Guards;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    internal class DownloaderBuilder : IDownloaderBuilder
    {
        #region Configure parameters

        private readonly HashSet<string> _remotePaths = new HashSet<string>();
        private string _localPath;

        private Action<TransferSettings, TransferContext> _settingsConfigurator;
        private Func<IEnumerable<string>, IRemotePathProvider> _remotePathProviderBuilder;
        private Func<long, IEnumerable<(long offset, long length)>> _blockIntervalGenerator;
        private Func<HttpWebRequest, HttpWebRequest> _requestInterceptor;
        private Func<string, string> _localPathInterceptor;
        private Interceptor<IObservable<BlockTransferContext>> _blockTransferItemInterceptor;

        #endregion

        public DownloaderBuilder()
        {
            ApplyDefaultConfigure();
        }

        private void ApplyDefaultConfigure()
        {
            _settingsConfigurator = (settings, context) => { };
            _remotePathProviderBuilder = remotePaths => new RemotePathProvider(remotePaths.ToList());
            _blockIntervalGenerator = GetBlockIntervals;
            _requestInterceptor = _ => _;
            _localPathInterceptor = _ => _;
            _blockTransferItemInterceptor = (input, context) => input;
        }

        #region Configure methods

        public IDownloaderBuilder From(string path)
        {
            ThrowIfNullReference(path);

            _remotePaths.Add(path);
            return this;
        }

        public IDownloaderBuilder From(IEnumerable<string> paths)
        {
            ThrowIfNullReference(paths);

            _remotePaths.UnionWith(paths);
            return this;
        }

        public IDownloaderBuilder To(string path)
        {
            if (_localPath != null)
                throw new InvalidOperationException("Cannot set local path repeatedly. ");

            ThrowIfNullReference(path);

            _localPath = path;

            return this;
        }

        public IDownloaderBuilder Configure(Action<TransferSettings, TransferContext> settingsConfigurator)
        {
            ThrowIfNullReference(settingsConfigurator);

            _settingsConfigurator = settingsConfigurator;
            return this;
        }

        public IDownloaderBuilder Configure(Func<IEnumerable<string>, IRemotePathProvider> remotePathProviderBuilder)
        {
            ThrowIfNullReference(remotePathProviderBuilder);

            _remotePathProviderBuilder = remotePathProviderBuilder;
            return this;
        }

        public IDownloaderBuilder Configure(Func<HttpWebRequest, HttpWebRequest> requestInterceptor)
        {
            ThrowIfNullReference(requestInterceptor);

            _requestInterceptor = requestInterceptor;
            return this;
        }

        public IDownloaderBuilder Configure(Func<string, string> localPathInterceptor)
        {
            ThrowIfNullReference(localPathInterceptor);

            _localPathInterceptor = localPathInterceptor;
            return this;
        }

        public IDownloaderBuilder Configure(Func<long, IEnumerable<(long offset, long length)>> blockIntervalGenerator)
        {
            ThrowIfNullReference(blockIntervalGenerator);

            _blockIntervalGenerator = blockIntervalGenerator;
            return this;
        }

        public IDownloaderBuilder Configure(Interceptor<IObservable<BlockTransferContext>> blockTransferItemInterceptor)
        {
            ThrowIfNullReference(blockTransferItemInterceptor);

            _blockTransferItemInterceptor = blockTransferItemInterceptor;
            return this;
        }

        #endregion

        public IDownloaderBuilder Clone()
        {
            var result = new DownloaderBuilder
            {
                _localPath = _localPath,

                _settingsConfigurator = _settingsConfigurator,
                _remotePathProviderBuilder = _remotePathProviderBuilder,
                _blockTransferItemInterceptor = _blockTransferItemInterceptor,
                _blockIntervalGenerator = _blockIntervalGenerator,
                _requestInterceptor = _requestInterceptor,
                _localPathInterceptor = _localPathInterceptor
            };

            result.From(_remotePaths);

            return result;
        }

        public IDownloader Build()
        {
            return new FileDownloader(BuildInternalAsync);
        }

        private async Task<IObservable<BlockTransferContext>> BuildInternalAsync(CancellationToken cancellationToken)
        {
            var context = new TransferContext
            {
                RemotePathProvider = _remotePathProviderBuilder(_remotePaths),
                LocalPath = _localPath
            };

            var settings = GetTransferSettings(context);

            var blockTransferContextGenerator = new Func<IRemotePathProvider, string>(provider => provider.GetRemotePath())
                .Then(PrimitiveMethods.ToRequest)
                .Then(_requestInterceptor)
                .Then(PrimitiveMethods.GetResponseAsync)
                .ThenAsync(response =>
                {
                    using (response)
                    {
                        return context.TotalSize = response.ContentLength;
                    }
                }, cancellationToken)
                .ThenAsync(_blockIntervalGenerator, cancellationToken)
                .ThenAsync(interval => new BlockTransferContext
                {
                    Offset = interval.offset,
                    TotalSize = interval.length,
                    RemotePath = context.RemotePathProvider.GetRemotePath(),
                    LocalPath = context.LocalPath
                }, cancellationToken);

            var blockDownloadItemFactory = new Func<BlockTransferContext, IObservable<BlockTransferContext>>(CreateBlockDownloadItem)
                .Then(item => _blockTransferItemInterceptor(item, context))
                .Then(settings.Handlers.ToInterceptor());

            settings.Handlers.BlockDownloadItemFactory = blockDownloadItemFactory;
            var blockDownloadItemsFactory = blockTransferContextGenerator.ThenAsync(blockDownloadItemFactory, cancellationToken);

            var blockDownloadTasks = await settings.BuildPolicy.ExecuteAsync(() =>
                blockDownloadItemsFactory(context.RemotePathProvider));

            return blockDownloadTasks.Merge(settings.MaxConcurrent);
        }

        private TransferSettings GetTransferSettings(TransferContext context)
        {
            var setting = new TransferSettings
            {
                BuildPolicy = BuildRunPolicy(context),
                MaxConcurrent = 16,
                Handlers = new BlockDownloadItemExceptionHandlers()
            };
            _settingsConfigurator(setting, context);

            return setting;
        }

        private Func<Task<(HttpWebResponse response, Stream inputStream)>> BuildStreamPairFatory(BlockTransferContext context)
        {
            return async () =>
            {
                var interval = (context.Offset + context.CompletedSize, context.TotalSize - context.CompletedSize);
                var request = context.RemotePath.ToRequest().Slice(interval);
                var response = await PrimitiveMethods.GetResponseAsync(request);

                var localStream = _localPathInterceptor(context.LocalPath).ToStream().Slice(interval);

                return (response, localStream);
            };
        }

        private IObservable<BlockTransferContext> CreateBlockDownloadItem(BlockTransferContext context)
        {
            return context.CompletedSize < context.TotalSize
                ? PrimitiveMethods.CreateBlockDownloadItem(BuildStreamPairFatory(context), context)
                : Observable.Empty<BlockTransferContext>();
        }

        private static IAsyncPolicy<IEnumerable<IObservable<BlockTransferContext>>> BuildRunPolicy(TransferContext context)
        {
            return Policy<IEnumerable<IObservable<BlockTransferContext>>>
                .Handle<WebException>()
                .RetryAsync(context.RemotePathProvider.RemotePaths.Count, (delegateResult, retryCount, policyContext) =>
                {
                    var remotePath = ((WebException)delegateResult.Exception).Response.ResponseUri.OriginalString;
                    context.RemotePathProvider.Vote(remotePath, -3);
                });
        }

        private static IEnumerable<(long Offset, long Length)> GetBlockIntervals(long totalLength)
        {
            const long blockLength = 1024 * 1024 * 20;

            long offset = 0;
            while (offset + blockLength < totalLength)
            {
                yield return (offset, blockLength);
                offset += blockLength;
            }

            yield return (offset, totalLength - offset);
        }
    }
}

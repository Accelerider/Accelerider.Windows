using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using static Accelerider.Windows.Infrastructure.Guards;


namespace Accelerider.Windows.Infrastructure.TransferService
{
    internal class FileDownloaderBuilder : IDownloaderBuilder
    {
        #region Configure parameters

        private Action<DownloadSettings, DownloadContext> _settingsConfigurator;
        private Func<HashSet<string>, IRemotePathProvider> _remotePathProviderBuilder;
        private Func<long, IEnumerable<(long offset, long length)>> _blockIntervalGenerator;
        private Func<HttpWebRequest, HttpWebRequest> _requestInterceptor;
        private Func<string, string> _localPathInterceptor;
        private Func<IObservable<(Guid Id, int Bytes)>, IObservable<(Guid Id, int Bytes)>> _blockTransferItemInterceptor;
        private Func<IDownloader, IDownloader> _postProcessInterceptor;

        #endregion

        public FileDownloaderBuilder()
        {
            ApplyDefaultConfigure();
        }

        private void ApplyDefaultConfigure()
        {
            _settingsConfigurator = (settings, context) => { };
            _remotePathProviderBuilder = remotePaths => new RemotePathProvider(remotePaths);
            _blockIntervalGenerator = size => new[] { (0L, size) };
            _requestInterceptor = _ => _;
            _localPathInterceptor = _ => _;
            _blockTransferItemInterceptor = _ => _;
            _postProcessInterceptor = _ => _;
        }

        #region Configure methods

        public IDownloaderBuilder Configure(Action<DownloadSettings, DownloadContext> settingsConfigurator)
        {
            ThrowIfNull(settingsConfigurator);

            _settingsConfigurator = settingsConfigurator;
            return this;
        }

        public IDownloaderBuilder Configure(Func<HashSet<string>, IRemotePathProvider> remotePathProviderBuilder)
        {
            ThrowIfNull(remotePathProviderBuilder);

            _remotePathProviderBuilder = remotePathProviderBuilder;
            return this;
        }

        public IDownloaderBuilder Configure(Func<HttpWebRequest, HttpWebRequest> requestInterceptor)
        {
            ThrowIfNull(requestInterceptor);

            _requestInterceptor = _requestInterceptor.Then(requestInterceptor);
            return this;
        }

        public IDownloaderBuilder Configure(Func<string, string> localPathInterceptor)
        {
            ThrowIfNull(localPathInterceptor);

            _localPathInterceptor = _localPathInterceptor.Then(localPathInterceptor);
            return this;
        }

        public IDownloaderBuilder Configure(Func<long, IEnumerable<(long offset, long length)>> blockIntervalGenerator)
        {
            ThrowIfNull(blockIntervalGenerator);

            _blockIntervalGenerator = blockIntervalGenerator;
            return this;
        }

        public IDownloaderBuilder Configure(Func<IObservable<(Guid Id, int Bytes)>, IObservable<(Guid Id, int Bytes)>> blockTransferItemInterceptor)
        {
            ThrowIfNull(blockTransferItemInterceptor);

            _blockTransferItemInterceptor = _blockTransferItemInterceptor.Then(blockTransferItemInterceptor);
            return this;
        }

        public IDownloaderBuilder Configure(Func<IDownloader, IDownloader> postProcessInterceptor)
        {
            ThrowIfNull(postProcessInterceptor);

            _postProcessInterceptor = _postProcessInterceptor.Then(postProcessInterceptor);
            return this;
        }

        #endregion

        public IDownloaderBuilder Clone()
        {
            var result = new FileDownloaderBuilder
            {
                _settingsConfigurator = _settingsConfigurator,
                _remotePathProviderBuilder = _remotePathProviderBuilder,
                _blockTransferItemInterceptor = _blockTransferItemInterceptor,
                _blockIntervalGenerator = _blockIntervalGenerator,
                _requestInterceptor = _requestInterceptor,
                _localPathInterceptor = _localPathInterceptor
            };

            return result;
        }

        public IDownloader Build()
        {
            var result = new FileDownloader(new FileDownloader.Builders
            {
                BlockTransferContextGeneratorBuilder = GetBlockTransferContextGenerator,
                BlockDownloadItemFactoryBuilder = GetBlockDownloadItemFactory,
                RemotePathProviderBuilder = _remotePathProviderBuilder,
                LocalPathInterceptor = _localPathInterceptor,
                TransferSettingsBuilder = GetTransferSettings
            });

            return _postProcessInterceptor(result);
        }

        private Func<CancellationToken, Task<IEnumerable<BlockTransferContext>>> GetBlockTransferContextGenerator(DownloadContext context)
        {
            return cancellationToken => new Func<IRemotePathProvider, string>(provider => provider.GetRemotePath())
                .Then(DownloadPrimitiveMethods.ToRequest)
                .Then(_requestInterceptor)
                .Then(DownloadPrimitiveMethods.GetResponseAsync)
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
                }, cancellationToken)
                .Invoke(context.RemotePathProvider);
        }

        private Func<BlockTransferContext, IObservable<(Guid Id, int Bytes)>> GetBlockDownloadItemFactory(DownloadSettings settings)
        {
            return new Func<BlockTransferContext, IObservable<(Guid Id, int Bytes)>>(CreateBlockDownloadItem)
                .Then(settings.DownloadPolicy.ExceptionInterceptor);
        }

        private DownloadSettings GetTransferSettings(DownloadContext context)
        {
            var setting = new DownloadSettings
            {
                BuildPolicy = Policy.NoOp(),
                MaxConcurrent = 1,
                DownloadPolicy = new BlockDownloadItemPolicy(CreateBlockDownloadItem)
            };
            _settingsConfigurator(setting, context);

            return setting;
        }

        private static Func<Task<(HttpWebResponse response, Stream inputStream)>> BuildStreamPairFatory(BlockTransferContext context)
        {
            return async () =>
            {
                var interval = (context.Offset + context.CompletedSize, context.TotalSize - context.CompletedSize);
                var request = context.RemotePath.ToRequest().Slice(interval);
                var response = await DownloadPrimitiveMethods.GetResponseAsync(request);

                var localStream = context.LocalPath.ToStream().Slice(interval);

                return (response, localStream);
            };
        }

        private IObservable<(Guid Id, int Bytes)> CreateBlockDownloadItem(BlockTransferContext context)
        {
            return context.CompletedSize < context.TotalSize
                ? new Func<BlockTransferContext, IObservable<(Guid Id, int Bytes)>>(
                        blockContext => DownloadPrimitiveMethods.CreateBlockDownloadItem(BuildStreamPairFatory(blockContext), blockContext))
                    .Then(_blockTransferItemInterceptor)
                    .Invoke(context)
                : Observable.Empty<(Guid Id, int Bytes)>();
        }
    }
}

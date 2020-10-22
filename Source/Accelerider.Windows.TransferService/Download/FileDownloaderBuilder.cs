using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Polly;

namespace Accelerider.Windows.TransferService
{
    internal class FileDownloaderBuilder : IDownloaderBuilder
    {
        #region Configure parameters

        private Action<DownloadSettings, DownloadContext> _settingsConfigurator;
        private Func<long, IEnumerable<(long offset, long length)>> _blockIntervalGenerator;
        private Func<IRemotePathProvider, IRemotePathProvider> _remotePathProviderInterceptor;
        private Func<HttpWebRequest, HttpWebRequest> _requestInterceptor;
        private Func<string, string> _localPathInterceptor;

        private IRemotePathProvider _remotePathProvider;
        private string _localPath;

        #endregion

        public FileDownloaderBuilder()
        {
            ApplyDefaultConfigure();
        }

        private void ApplyDefaultConfigure()
        {
            _settingsConfigurator = (settings, context) => { };
            _blockIntervalGenerator = size => new[] { (0L, size) };
            _remotePathProviderInterceptor = _ => _;
            _requestInterceptor = _ => _;
            _localPathInterceptor = _ => _;
        }

        #region Configure methods

        public IDownloaderBuilder Configure(Action<DownloadSettings, DownloadContext> settingsConfigurator)
        {
            Guards.ThrowIfNull(settingsConfigurator);

            _settingsConfigurator = settingsConfigurator;
            return this;
        }

        public IDownloaderBuilder Configure(Func<HttpWebRequest, HttpWebRequest> requestInterceptor)
        {
            Guards.ThrowIfNull(requestInterceptor);

            _requestInterceptor = _requestInterceptor.Then(requestInterceptor);
            return this;
        }

        public IDownloaderBuilder Configure(Func<string, string> localPathInterceptor)
        {
            Guards.ThrowIfNull(localPathInterceptor);

            _localPathInterceptor = _localPathInterceptor.Then(localPathInterceptor);
            return this;
        }

        public IDownloaderBuilder Configure(Func<long, IEnumerable<(long Offset, long Length)>> blockIntervalGenerator)
        {
            Guards.ThrowIfNull(blockIntervalGenerator);

            _blockIntervalGenerator = blockIntervalGenerator;
            return this;
        }

        public IDownloaderBuilder Configure<T>(Func<T, IRemotePathProvider> remotePathProviderInterceptor) where T : IRemotePathProvider
        {
            Guards.ThrowIfNull(remotePathProviderInterceptor);

            _remotePathProviderInterceptor = _remotePathProviderInterceptor
                .Then(item => remotePathProviderInterceptor((T)item));
            return this;
        }

        public IDownloaderBuilder From(IRemotePathProvider provider)
        {
            Guards.ThrowIfNull(provider);

            _remotePathProvider = provider;
            return this;
        }

        public IDownloaderBuilder To(string path)
        {
            Guards.ThrowIfNullOrEmpty(path);

            if (!path.Equals(_localPath, StringComparison.InvariantCultureIgnoreCase))
            {
                _localPath = path;
            }

            return this;
        }

        #endregion

        public IDownloaderBuilder Clone()
        {
            var result = new FileDownloaderBuilder
            {
                _settingsConfigurator = _settingsConfigurator,
                _blockIntervalGenerator = _blockIntervalGenerator,
                _remotePathProviderInterceptor = _remotePathProviderInterceptor,
                _requestInterceptor = _requestInterceptor,
                _localPathInterceptor = _localPathInterceptor
            };

            return result;
        }

        public IDownloader Build()
        {
            var context = new DownloadContext(Guid.NewGuid())
            {
                RemotePathProvider = _remotePathProviderInterceptor(_remotePathProvider),
                LocalPath = _localPathInterceptor(_localPath)
            };

            return InternalBuild(context, GetBlockTransferContextGenerator);
        }

        public IDownloader Build(DownloadContext context, IEnumerable<BlockTransferContext> blockContexts)
        {
            Guards.ThrowIfNull(context);

            if (context.RemotePathProvider == null)
            {
                context.RemotePathProvider = _remotePathProvider ?? throw new ArgumentException("The Context.RemotePathProvider cannot be null");
            }

            context.RemotePathProvider = _remotePathProviderInterceptor(context.RemotePathProvider);

            var blockContextsArray = blockContexts?.ToArray();
            return blockContextsArray != null && blockContextsArray.Any()
                ? InternalBuild(context, ctx => token =>
                {
                    blockContextsArray.ForEach(item =>
                    {
                        item.LocalPath = ctx.LocalPath;
                        item.RemotePathGetter = ctx.RemotePathProvider.GetAsync;
                    });
                    return Task.FromResult(blockContextsArray.AsEnumerable());
                })
                : InternalBuild(context, GetBlockTransferContextGenerator);
        }

        private IDownloader InternalBuild(DownloadContext context, Func<DownloadContext, Func<CancellationToken, Task<IEnumerable<BlockTransferContext>>>> blockTransferContextGeneratorBuilder)
        {
            var settings = GetTransferSettings(context);

            var result = new FileDownloader(new FileDownloader.BuildInfo
            {
                Context = context,
                Settings = settings,
                BlockTransferContextGeneratorBuilder = blockTransferContextGeneratorBuilder,
                BlockDownloadItemFactoryBuilder = GetBlockDownloadItemFactory
            });

            return result;
        }

        private Func<CancellationToken, Task<IEnumerable<BlockTransferContext>>> GetBlockTransferContextGenerator(DownloadContext context)
        {
            return cancellationToken => new Func<IRemotePathProvider, Task<string>>(provider => provider.GetAsync())
                .ThenAsync(DownloadPrimitiveMethods.ToRequest)
                .ThenAsync(request =>
                {
                    cancellationToken.Register(request.Abort);
                    return request;
                })
                .ThenAsync(_requestInterceptor)
                .ThenAsync(DownloadPrimitiveMethods.GetResponseAsync)
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
                    RemotePathGetter = context.RemotePathProvider.GetAsync,
                    LocalPath = context.LocalPath
                }, cancellationToken)
                .Invoke(context.RemotePathProvider);
        }

        private Func<BlockTransferContext, IObservable<(long Offset, int Bytes)>> GetBlockDownloadItemFactory(DownloadSettings settings)
        {
            return new Func<BlockTransferContext, IObservable<(long Offset, int Bytes)>>(CreateBlockDownloadItem)
                .Then(settings.DownloadPolicy.ExceptionInterceptor);
        }

        private DownloadSettings GetTransferSettings(DownloadContext context)
        {
            var setting = new DownloadSettings
            {
                BuildPolicy = Policy.NoOpAsync(),
                MaxConcurrent = 1,
                DownloadPolicy = new BlockDownloadItemPolicy(CreateBlockDownloadItem)
            };
            _settingsConfigurator(setting, context);

            return setting;
        }

        private IObservable<(long Offset, int Bytes)> CreateBlockDownloadItem(BlockTransferContext context)
        {
            return context.CompletedSize < context.TotalSize
                ? new Func<BlockTransferContext, IObservable<(long Offset, int Bytes)>>(
                        blockContext => DownloadPrimitiveMethods.CreateBlockDownloadItem(BuildStreamPairFactory(blockContext), blockContext))
                    .Invoke(context)
                : Observable.Empty<(long Offset, int Bytes)>();
        }

        private Func<Task<(HttpWebResponse response, Stream inputStream)>> BuildStreamPairFactory(BlockTransferContext context)
        {
            return async () =>
            {
                var interval = (context.Offset + context.CompletedSize, context.TotalSize - context.CompletedSize);
                var remotePath = await context.RemotePathGetter();
                var request = _requestInterceptor(remotePath.ToRequest()).Slice(interval);
                var response = await DownloadPrimitiveMethods.GetResponseAsync(request);

                var localStream = context.LocalPath.EnsureFileFolder().ToStream().Slice(interval);

                return (response, localStream);
            };
        }
    }
}

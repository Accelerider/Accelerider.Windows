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
        private Func<HashSet<string>, IRemotePathProvider> _remotePathProviderBuilder;
        private Func<long, IEnumerable<(long offset, long length)>> _blockIntervalGenerator;
        private Func<HttpWebRequest, HttpWebRequest> _requestInterceptor;
        private Func<string, string> _localPathInterceptor;
        private Func<IObservable<(Guid Id, int Bytes)>, IObservable<(Guid Id, int Bytes)>> _blockTransferItemInterceptor;
        private Func<IDownloader, IDownloader> _postProcessInterceptor;

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
            _requestInterceptor = _ => _;
            _localPathInterceptor = _ => _;
            _blockTransferItemInterceptor = _ => _;
            _postProcessInterceptor = _ => _;
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

        public IDownloaderBuilder Configure(Func<long, IEnumerable<(long offset, long length)>> blockIntervalGenerator)
        {
            Guards.ThrowIfNull(blockIntervalGenerator);

            _blockIntervalGenerator = blockIntervalGenerator;
            return this;
        }

        public IDownloaderBuilder Configure(Func<IObservable<(Guid Id, int Bytes)>, IObservable<(Guid Id, int Bytes)>> blockTransferItemInterceptor)
        {
            Guards.ThrowIfNull(blockTransferItemInterceptor);

            _blockTransferItemInterceptor = _blockTransferItemInterceptor.Then(blockTransferItemInterceptor);
            return this;
        }

        public IDownloaderBuilder Configure(Func<IDownloader, IDownloader> postProcessInterceptor)
        {
            Guards.ThrowIfNull(postProcessInterceptor);

            _postProcessInterceptor = _postProcessInterceptor.Then(postProcessInterceptor);
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
            var context = new DownloadContext(Guid.NewGuid())
            {
                RemotePathProvider = _remotePathProvider,
                LocalPath = _localPathInterceptor(_localPath)
            };

            return InternalBuild(context, GetBlockTransferContextGenerator);
        }

        public IDownloader Build(string json)
        {
            Guards.ThrowIfNullOrEmpty(json);

            var serializedData = json.ToObject<DownloaderSerializedData>();

            if (serializedData?.Context == null || serializedData.BlockContexts == null)
                throw new InvalidOperationException();

            var context = serializedData.Context;

            Configure(downloader =>
            {
                downloader.Tag = serializedData.Tag;
                return downloader;
            });

            return InternalBuild(context, ctx => token =>
            {
                serializedData.BlockContexts.ForEach(item => item.LocalPath = ctx.LocalPath);
                serializedData.BlockContexts.ForEach(item => item.RemotePathGetter = ctx.RemotePathProvider.GetRemotePathAsync);
                return Task.FromResult(serializedData.BlockContexts.AsEnumerable());
            });
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

            return _postProcessInterceptor(result);
        }

        private Func<CancellationToken, Task<IEnumerable<BlockTransferContext>>> GetBlockTransferContextGenerator(DownloadContext context)
        {
            return cancellationToken => new Func<IRemotePathProvider, Task<string>>(provider => provider.GetRemotePathAsync())
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
                    RemotePathGetter = context.RemotePathProvider.GetRemotePathAsync,
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

        private IObservable<(Guid Id, int Bytes)> CreateBlockDownloadItem(BlockTransferContext context)
        {
            return context.CompletedSize < context.TotalSize
                ? new Func<BlockTransferContext, IObservable<(Guid Id, int Bytes)>>(
                        blockContext => DownloadPrimitiveMethods.CreateBlockDownloadItem(BuildStreamPairFactory(blockContext), blockContext))
                    .Then(_blockTransferItemInterceptor)
                    .Invoke(context)
                : Observable.Empty<(Guid Id, int Bytes)>();
        }

        private Func<Task<(HttpWebResponse response, Stream inputStream)>> BuildStreamPairFactory(BlockTransferContext context)
        {
            return async () =>
            {
                var interval = (context.Offset + context.CompletedSize, context.TotalSize - context.CompletedSize);
                var remotePath = await context.RemotePathGetter();
                var request = _requestInterceptor(remotePath.ToRequest()).Slice(interval);
                var response = await DownloadPrimitiveMethods.GetResponseAsync(request);

                var localStream = context.LocalPath.ToStream().Slice(interval);

                return (response, localStream);
            };
        }
    }
}

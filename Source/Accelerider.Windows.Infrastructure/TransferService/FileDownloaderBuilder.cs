using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private readonly HashSet<string> _remotePaths = new HashSet<string>();
        private string _localPath;

        private Action<TransferSettings, TransferContext> _settingsConfigurator;
        private Func<IEnumerable<string>, IRemotePathProvider> _remotePathProviderBuilder;
        private Func<long, IEnumerable<(long offset, long length)>> _blockIntervalGenerator;
        private Func<HttpWebRequest, HttpWebRequest> _requestInterceptor;
        private Func<string, string> _localPathInterceptor;
        private Func<IObservable<BlockTransferContext>, IObservable<BlockTransferContext>> _blockTransferItemInterceptor;

        #endregion

        public FileDownloaderBuilder()
        {
            ApplyDefaultConfigure();
        }

        private void ApplyDefaultConfigure()
        {
            _settingsConfigurator = (settings, context) => { };
            _remotePathProviderBuilder = remotePaths => new RemotePathProvider(remotePaths.ToList());
            _blockIntervalGenerator = size => new[] { (0L, size) };
            _requestInterceptor = _ => _;
            _localPathInterceptor = _ => _;
            _blockTransferItemInterceptor = _ => _;
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

            _requestInterceptor = _requestInterceptor.Then(requestInterceptor);
            return this;
        }

        public IDownloaderBuilder Configure(Func<string, string> localPathInterceptor)
        {
            ThrowIfNullReference(localPathInterceptor);

            _localPathInterceptor = _localPathInterceptor.Then(localPathInterceptor);
            return this;
        }

        public IDownloaderBuilder Configure(Func<long, IEnumerable<(long offset, long length)>> blockIntervalGenerator)
        {
            ThrowIfNullReference(blockIntervalGenerator);

            _blockIntervalGenerator = blockIntervalGenerator;
            return this;
        }

        public IDownloaderBuilder Configure(Func<IObservable<BlockTransferContext>, IObservable<BlockTransferContext>> blockTransferItemInterceptor)
        {
            ThrowIfNullReference(blockTransferItemInterceptor);

            _blockTransferItemInterceptor = _blockTransferItemInterceptor.Then(blockTransferItemInterceptor);
            return this;
        }

        #endregion

        public IDownloaderBuilder Clone()
        {
            var result = new FileDownloaderBuilder
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
            var context = new TransferContext
            {
                RemotePathProvider = _remotePathProviderBuilder(_remotePaths),
                LocalPath = _localPathInterceptor(_localPath)
            };
            var settings = GetTransferSettings(context);

            return new FileDownloader(
                GetBlockTransferContextGenerator(context),
                GetBlockDownloadItemFactory(settings),
                context,
                settings);
        }

        private Func<CancellationToken, Task<IEnumerable<BlockTransferContext>>> GetBlockTransferContextGenerator(TransferContext context)
        {
            return cancellationToken => new Func<IRemotePathProvider, string>(provider => provider.GetRemotePath())
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
                }, cancellationToken)
                .Invoke(context.RemotePathProvider);
        }

        private Func<BlockTransferContext, IObservable<BlockTransferContext>> GetBlockDownloadItemFactory(TransferSettings settings)
        {
            return new Func<BlockTransferContext, IObservable<BlockTransferContext>>(CreateBlockDownloadItem)
                .Then(settings.DownloadPolicy.ToInterceptor());
        }

        private TransferSettings GetTransferSettings(TransferContext context)
        {
            var setting = new TransferSettings
            {
                BuildPolicy = Policy.NoOp(),
                MaxConcurrent = 1,
                DownloadPolicy = new BlockDownloadItemPolicy(CreateBlockDownloadItem)
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

                var localStream = context.LocalPath.ToStream().Slice(interval);

                return (response, localStream);
            };
        }

        private IObservable<BlockTransferContext> CreateBlockDownloadItem(BlockTransferContext context)
        {
            return context.CompletedSize < context.TotalSize
                ? new Func<BlockTransferContext, IObservable<BlockTransferContext>>(
                        blockContext => PrimitiveMethods.CreateBlockDownloadItem(BuildStreamPairFatory(blockContext), blockContext))
                    .Then(_blockTransferItemInterceptor)
                    .Invoke(context)
                : Observable.Empty<BlockTransferContext>();
        }
    }
}

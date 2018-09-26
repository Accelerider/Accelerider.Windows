using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Polly;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    public static class FileTransferService
    {
        private const long BlockLength = 1024 * 1024 * 20;

        static FileTransferService()
        {
            ServicePointManager.DefaultConnectionLimit = 99999;
        }

        public static async Task<(IConnectableObservable<BlockTransferContext> observable, TransferContext context)> DownloadAsync(
            List<string> remotePaths,
            string localPath,
            CancellationToken cancellationToken,
            IAsyncPolicy<IEnumerable<IObservable<BlockTransferContext>>> runPolicy = null,
            int maxConcurrent = 4)
        {
            var context = new TransferContext
            {
                RemotePathProvider = new RemotePathProvider(remotePaths),
                LocalPath = localPath
            };

            var blockDownloadItemsFactory = BuildBlockDownloadItemsFactory(context, cancellationToken);

            runPolicy = runPolicy ?? BuildRunPolicy(context);
            var blockDownloadTasks = await runPolicy.ExecuteAsync(policyContext => blockDownloadItemsFactory(policyContext.GetRemotePathProvider()), new Dictionary<string, object>
            {
                { nameof(IRemotePathProvider), context.RemotePathProvider }
            });

            return (blockDownloadTasks.Merge(maxConcurrent).Publish(), context);
        }

        private static IAsyncPolicy<IEnumerable<IObservable<BlockTransferContext>>> BuildRunPolicy(TransferContext context)
        {
            return Policy<IEnumerable<IObservable<BlockTransferContext>>>
                .Handle<WebException>()
                .RetryAsync(context.RemotePathProvider.RemotePaths.Count, (delegateResult, retryCount, policyContext) =>
                {
                    var remotePath = ((WebException)delegateResult.Exception).Response.ResponseUri.OriginalString;
                    var provider = policyContext.GetRemotePathProvider();
                    provider.Vote(remotePath, -3);
                });
        }

        private static Func<IRemotePathProvider, Task<IEnumerable<IObservable<BlockTransferContext>>>> BuildBlockDownloadItemsFactory(TransferContext context, CancellationToken cancellationToken)
        {
            return new Func<IRemotePathProvider, string>(provider => provider.GetRemotePath())
                .Then(Primitives.GetRequest)
                .Then(Primitives.GetResponseAsync)
                .ThenAsync(response =>
                {
                    using (response)
                    {
                        return context.TotalSize = response.ContentLength;
                    }
                }, cancellationToken)
                .ThenAsync(GetBlockIntervals, cancellationToken)
                .ThenAsync(((long offset, long length) interval) => new BlockTransferContext
                {
                    Offset = interval.offset,
                    TotalSize = interval.length,
                    RemotePath = context.RemotePathProvider.GetRemotePath(),
                    LocalPath = context.LocalPath
                }, cancellationToken)
                .ThenAsync(CreateBlockDownloadItem, cancellationToken)
                .ThenAsync(item => item
                        .Catch<BlockTransferException>(e => HandleBlockDownloadItemException(e, context.RemotePathProvider))
                        .Catch<RemotePathExhaustedException>(e => Observable.Empty<BlockTransferContext>())
                        .Catch<OperationCanceledException>(e => Observable.Empty<BlockTransferContext>()),
                    cancellationToken);
        }

        public static IObservable<BlockTransferContext> CreateBlockDownloadItem(BlockTransferContext blockContext)
        {
            var streamPairFatory = new Func<Task<(HttpWebResponse response, Stream inputStream)>>(async () =>
            {
                var interval = (blockContext.Offset + blockContext.CompletedSize, blockContext.TotalSize - blockContext.CompletedSize);
                var request = blockContext.RemotePath.GetRequest().GetRequest(interval);
                var response = await Primitives.GetResponseAsync(request);

                var localStream = blockContext.LocalPath.GetStream(interval);

                return (response, localStream);
            });

            return Primitives.CreateBlockDownloadItem(streamPairFatory, blockContext);
        }


        private static IObservable<BlockTransferContext> HandleBlockDownloadItemException(BlockTransferException exception, IRemotePathProvider remotePathProvider)
        {
            exception.Context.Bytes = 0;
            exception.Context.RemotePath = remotePathProvider.GetRemotePath();
            return CreateBlockDownloadItem(exception.Context)
                .Catch<BlockTransferException>(e => HandleBlockDownloadItemException(e, remotePathProvider));
        }

        public static IEnumerable<(long Offset, long Length)> GetBlockIntervals(long totalLength)
        {
            long offset = 0;
            while (offset + BlockLength < totalLength)
            {
                yield return (offset, BlockLength);
                offset += BlockLength;
            }

            yield return (offset, totalLength - offset);
        }

        public static IRemotePathProvider GetRemotePathProvider(this Context @this)
        {
            return (IRemotePathProvider)@this[nameof(IRemotePathProvider)];
        }
    }
}

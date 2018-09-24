using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xaml;
using Accelerider.Windows.Infrastructure.FileTransferService;
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

        public static async Task<(IObservable<BlockTransferContext> observable, TransferContext context)> DownloadAsync(
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

            var blockDownloadTasksFactory = BuildBlockDownloadItemsFactory(context, cancellationToken);

            runPolicy = runPolicy ?? BuildRunPolicy(context);
            var blockDownloadTasks = await runPolicy.ExecuteAsync(policyContext => blockDownloadTasksFactory(policyContext.GetRemotePathProvider()), new Dictionary<string, object>
            {
                { nameof(IRemotePathProvider), context.RemotePathProvider }
            });

            var observable = RunBlockDownloadItems(blockDownloadTasks.Merge(maxConcurrent), cancellationToken);

            return (observable, context);
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

        private static Func<IRemotePathProvider, Task<IEnumerable<IObservable<BlockTransferContext>>>> BuildBlockDownloadItemsFactory(
            TransferContext context,
            CancellationToken cancellationToken)
        {
            var generateBlockContext = new Func<IRemotePathProvider, string>(provider => provider.GetRemotePath())
                .Then(CreateRequest)
                .Then(GetResponseAsync)
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
                }, cancellationToken);


            return generateBlockContext.ThenAsync(blockContext => CreateBlockDownloadItem(blockContext)
                .Catch<BlockTransferContext, BlockTransferException>(e => HandleBlockDownloadItemException(e, context.RemotePathProvider)));
        }

        private static IObservable<BlockTransferContext> HandleBlockDownloadItemException(BlockTransferException exception, IRemotePathProvider remotePathProvider)
        {
            Console.WriteLine($"[Error] {exception.Context.Id:B}, CompletedSize: {exception.Context.CompletedSize}, Ex: {exception.InnerException.GetType().Name}");
            exception.Context.Bytes = 0;
            exception.Context.RemotePath = remotePathProvider.GetRemotePath();
            return CreateBlockDownloadItem(exception.Context)
                .Catch<BlockTransferContext, BlockTransferException>(e => HandleBlockDownloadItemException(e, remotePathProvider));
        }


        private static IObservable<BlockTransferContext> CreateBlockDownloadItem(BlockTransferContext blockContext)
        {
            var streamPairFatory = new Func<Task<(HttpWebResponse response, Stream inputStream)>>(async () =>
            {
                var interval = (blockContext.Offset + blockContext.CompletedSize, blockContext.TotalSize - blockContext.CompletedSize);
                var request = GetBlockRequest(CreateRequest(blockContext.RemotePath), interval);
                var response = await GetResponseAsync(request);

                var localStream = CreateBlockLocalStream(blockContext.LocalPath, interval);

                return (response, localStream);
            });

            return CreateBlockDownloadItem(streamPairFatory, blockContext);
        }

        private static IObservable<BlockTransferContext> RunBlockDownloadItems(IObservable<BlockTransferContext> observable, CancellationToken cancellationToken)
        {
            var observerList = new ObserverList<BlockTransferContext>();

            var disposable = observable.Subscribe(observerList);
            cancellationToken.Register(disposable.Dispose);

            return Observable.Create<BlockTransferContext>(o =>
            {
                observerList.Add(o);
                return () => observerList.Remove(o);
            });
        }

        public static HttpWebRequest CreateRequest(string remotePath)
        {
            return WebRequest.CreateHttp(remotePath);
        }

        public static async Task<HttpWebResponse> GetResponseAsync(HttpWebRequest request)
        {
            return (HttpWebResponse)await request.GetResponseAsync();
        }

        public static Stream GetRemoteStream(HttpWebResponse response)
        {
            return response.GetResponseStream();
        }

        public static Stream CreateBlockLocalStream(string localPath, (long offset, long length) block = default)
        {
            var stream = File.Open(localPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
            stream.Position = block.offset;
            return stream;
        }

        public static HttpWebRequest GetBlockRequest(HttpWebRequest request, (long offset, long length) block)
        {
            request.AddRangeBasedOffsetLength(block.offset, block.length);
            return request;
        }

        private static int _count = 0;

        public static IObservable<BlockTransferContext> CreateBlockDownloadItem(
            Func<Task<(HttpWebResponse response, Stream inputStream)>> streamPairFactory,
            BlockTransferContext context) => Observable.Create<BlockTransferContext>(o =>
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            // 2. Execute copy stream by async.
            Task.Run(async () =>
            {
                try
                {
                    (HttpWebResponse response, Stream inputStream) = await streamPairFactory();

                    using (response)
                    using (var outputStream = GetRemoteStream(response))
                    using (inputStream)
                    {
                        byte[] buffer = new byte[128 * 1024];
                        int count;
                        context.Status = TransferStatus.Transferring;
                        o.OnNext(context);
                        while ((count = await outputStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            await inputStream.WriteAsync(buffer, 0, count, cancellationToken);
                            context.Bytes = count;
                            o.OnNext(context);

                            if (context.Offset / BlockLength == 1)
                            {
                                if (_count == 0)
                                {
                                    _count++;
                                    throw new WebException();
                                }
                                if (_count == 1)
                                {
                                    _count++;
                                    Console.WriteLine($"[Restart] {context.Id:B}");
                                }
                            }
                        }
                    }

                    o.OnCompleted();
                }
                catch (Exception e)
                {
                    o.OnError(new BlockTransferException(context, e));
                }
            }, cancellationToken);

            return () => cancellationTokenSource.Cancel();
        });

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

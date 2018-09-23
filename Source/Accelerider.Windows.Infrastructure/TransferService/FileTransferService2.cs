using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.FileTransferService;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    public class FileTransferService2
    {
        private const long BlockLength = 1024 * 1024 * 20;

        static FileTransferService2()
        {
            ServicePointManager.DefaultConnectionLimit = 99999;
        }

        public static async Task<IObservable<TransferContext>> CreateDownloadTaskAsync(string remotePath, string localPath, CancellationToken cancellationToken, int maxConcurrent = 4)
        {
            var context = new TransferContext
            {
                RemotePath = remotePath,
                LocalPath = localPath
            };
            cancellationToken.Register(() => context.Status = TransferStatus.Disposed);

            if (cancellationToken.IsCancellationRequested) return Observable.Empty<TransferContext>();
            using (var response = await GetResponseAsync(CreateRequest(remotePath)))
            {
                context.TotalSize = response.ContentLength;
            }

            if (cancellationToken.IsCancellationRequested) return Observable.Empty<TransferContext>();
            var observable = Split(context.TotalSize)
                .Select(interval =>
                {
                    var streamPairFactory = new Func<Task<(HttpWebResponse response, Stream localStream)>>(async () =>
                    {
                        var request = GetBlockRequest(CreateRequest(remotePath), interval);
                        var response = await GetResponseAsync(request);

                        var localStream = CreateStream(localPath, interval);

                        return (response, localStream);
                    });

                    return (streamPairFactory, interval);
                })
                .Select(parameter => CreateBlockDownloadTask(parameter.streamPairFactory, parameter.interval))
                .Merge(maxConcurrent)
                .Select(blockContext =>
                {
                    context.Status = TransferStatus.Transferring;
                    context.CompletedSize += blockContext.Bytes;
                    return context;
                });

            return CreateDownloadObservable(observable, cancellationToken);
        }

        private static IObservable<TransferContext> CreateDownloadObservable(IObservable<TransferContext> observable, CancellationToken cancellationToken)
        {
            var observerList = new ObserverList<TransferContext>();

            var disposable = observable.Subscribe(observerList);
            cancellationToken.Register(disposable.Dispose);

            return Observable.Create<TransferContext>(o =>
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

        public static Stream GetStream(HttpWebResponse response)
        {
            return response.GetResponseStream();
        }

        public static Stream CreateStream(string localPath, (long offset, long length) block = default)
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

        public static IObservable<BlockTransferContext> CreateBlockDownloadTask(
            Func<Task<(HttpWebResponse response, Stream inputStream)>> streamPairFactory,
            (long offset, long length) blockInterval) => Observable.Create<BlockTransferContext>(o =>
        {
            // 1. Initialize context.
            var context = new BlockTransferContext
            {
                Offset = blockInterval.offset,
                TotalSize = blockInterval.length
            };
            var source = new CancellationTokenSource();
            var token = source.Token;

            // 2. Execute copy stream by async.
            Task.Run(async () =>
            {
                try
                {
                    (HttpWebResponse response, Stream inputStream) = await streamPairFactory();

                    using (response)
                    using (var outputStream = GetStream(response))
                    using (inputStream)
                    {
                        byte[] buffer = new byte[128 * 1024];
                        int count;
                        context.Status = TransferStatus.Transferring;
                        o.OnNext(context);
                        while ((count = await outputStream.ReadAsync(buffer, 0, buffer.Length, token)) > 0)
                        {
                            if (token.IsCancellationRequested) break;
                            await inputStream.WriteAsync(buffer, 0, count, token);
                            context.Bytes = count;
                            o.OnNext(context);
                        }
                    }

                    o.OnCompleted();
                }
                catch (TaskCanceledException)
                {
                    o.OnCompleted();
                }
                catch (Exception)
                {
                    context.Status = TransferStatus.Faulted;
                    o.OnNext(context);
                }
            }, token);

            return () => source.Cancel();
        });

        public static IEnumerable<(long Offset, long Length)> Split(long totalLength)
        {
            long offset = 0;
            while (offset + BlockLength < totalLength)
            {
                yield return (offset, BlockLength);
                offset += BlockLength;
            }

            yield return (offset, totalLength - offset);
        }
    }
}

using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    public static class Primitives
    {
        [Pure]
        public static HttpWebRequest GetRequest(this string remotePath)
        {
            return WebRequest.CreateHttp(remotePath);
        }

        [Pure]
        public static HttpWebRequest GetRequest(this HttpWebRequest request, (long offset, long length) block)
        {
            request.AddRangeBasedOffsetLength(block.offset, block.length);
            return request;
        }

        [Pure]
        public static async Task<HttpWebResponse> GetResponseAsync(HttpWebRequest request)
        {
            return (HttpWebResponse)await request.GetResponseAsync();
        }

        [Pure]
        public static Stream GetStream(this HttpWebResponse response)
        {
            return response.GetResponseStream();
        }

        [Pure]
        public static Stream GetStream(this string localPath, (long offset, long length) block = default)
        {
            var stream = File.Open(localPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
            stream.Position = block.offset;
            return stream;
        }

        [Pure]
        public static IObservable<BlockTransferContext> CreateBlockDownloadItem(Func<Task<(HttpWebResponse response, Stream inputStream)>> streamPairFactory, BlockTransferContext context) => Observable.Create<BlockTransferContext>(o =>
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            // Execute copy stream by async.
            Task.Run(async () =>
            {
                try
                {
                    (HttpWebResponse response, Stream inputStream) = await streamPairFactory();

                    using (response)
                    using (var outputStream = response.GetStream())
                    using (inputStream)
                    {
                        //context.Status = TransferStatus.Transferring;
                        //o.OnNext(context);

                        byte[] buffer = new byte[128 * 1024];
                        int count;
                        while ((count = await outputStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            await inputStream.WriteAsync(buffer, 0, count, cancellationToken);
                            context.Bytes = count;
                            o.OnNext(context);
                        }
                    }

                    o.OnCompleted();
                }
                catch (OperationCanceledException e)
                {
                    o.OnError(e);
                }
                catch (Exception e)
                {
                    o.OnError(new BlockTransferException(context, e));
                }
            }, cancellationToken);

            return () =>
            {
                Debug.WriteLine($"{context.Id} has been disposed. ");
                cancellationTokenSource.Cancel();
            };
        });

        public static IObservable<BlockTransferContext> Catch<TException>(
            this IObservable<BlockTransferContext> @this,
            Func<TException, IObservable<BlockTransferContext>> handler)
            where TException : Exception
        {
            return @this.Catch<BlockTransferContext, TException>(handler);
        }
    }
}

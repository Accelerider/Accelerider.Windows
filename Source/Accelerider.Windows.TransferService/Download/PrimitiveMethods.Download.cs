using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Accelerider.Windows.TransferService
{
    public static class DownloadPrimitiveMethods
    {
        public static HttpWebRequest ToRequest(this string remotePath)
        {
            return WebRequest.CreateHttp(remotePath);
        }

        public static HttpWebRequest Slice(this HttpWebRequest request, (long offset, long length) block)
        {
            request.AddRangeBasedOffsetLength(block.offset, block.length);
            return request;
        }

        public static async Task<HttpWebResponse> GetResponseAsync(HttpWebRequest request)
        {
            return (HttpWebResponse)await request.GetResponseAsync();
        }

        public static FileStream ToStream(this string localPath)
        {
            var folderPath = Path.GetDirectoryName(localPath) ?? throw new InvalidOperationException();
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            return File.Open(localPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
        }

        public static FileStream Slice(this FileStream stream, (long offset, long length) block)
        {
            stream.Position = block.offset;
            return stream;
        }

        public static IObservable<(long Offset, int Bytes)> CreateBlockDownloadItem(
            Func<Task<(HttpWebResponse response, Stream inputStream)>> streamPairFactory, 
            BlockTransferContext context) => Observable.Create<(long Offset, int Bytes)>(o =>
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
                    using (var outputStream = response.GetResponseStream())
                    using (inputStream)
                    {
                        byte[] buffer = new byte[128 * 1024];
                        int count;
                        while (outputStream != null && (count = await outputStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            await inputStream.WriteAsync(buffer, 0, count, cancellationToken);
                            o.OnNext((context.Offset, count));
                        }
                    }

                    o.OnCompleted();
                }
                catch (Exception e)
                {
                    o.OnError(new BlockTransferException(context, e));
                }
            }, cancellationToken);

            return () =>
            {
                Debug.WriteLine($"{context.Offset} has been disposed. ");
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

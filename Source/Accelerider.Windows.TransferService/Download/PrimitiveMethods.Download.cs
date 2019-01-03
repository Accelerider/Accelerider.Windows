using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;

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
            return new FileStream(localPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write, 1024 * 1024);
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
                    (HttpWebResponse response, Stream outputStream) = await streamPairFactory();

                    using (response)
                    using (var inputStream = response.GetResponseStream())
                    using (outputStream)
                    {
                        byte[] buffer = new byte[128 * 1024];
                        int count;

                        Guards.ThrowIfNull(inputStream);

                        // ReSharper disable once PossibleNullReferenceException
                        while ((count = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            if (cancellationToken.IsCancellationRequested)
                            {
                                Debug.WriteLine($"[CANCELLED] [{DateTime.Now}] BLOCK DOWNLOAD ITEM ({context.Offset})");
                                o.OnError(new BlockTransferException(context, new OperationCanceledException()));
                                return;
                            }

                            outputStream.Write(buffer, 0, count);
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
                Debug.WriteLine($"[DISPOSED] [{DateTime.Now}] BLOCK DOWNLOAD ITEM ({context.Offset})");
                cancellationTokenSource.Cancel();
            };
        });
    }
}

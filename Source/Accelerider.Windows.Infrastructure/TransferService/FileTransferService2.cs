using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    public static class FileTransferService2
    {
        private const long BlockLength = 1024 * 1024 * 10;

        static FileTransferService2()
        {
            ServicePointManager.DefaultConnectionLimit = 99999;
        }

        public static async Task BlockDownloadAsync(string remotePath, string localPath, CancellationToken token)
        {
            if (token.IsCancellationRequested) return;
            var totalSize = (await GetResponseAsync(CreateRequest(remotePath))).ContentLength;
            var blocks = Split(totalSize, BlockLength).ToArray();

            if (token.IsCancellationRequested) return;
            long completedSize = 0;
            var observables = blocks
                .Select(item => GetBlockRequest(CreateRequest(remotePath), item))
                .Select(GetResponseAsync)
                .Select(async item => GetStream(await item))
                .Zip(blocks.Select(item => CreateStream(localPath, item)), (remoteStream, localStream) => (remoteStream, localStream))
                .Select(item => CopyStream(item.remoteStream, item.localStream, token));

            var task = observables
                .Merge(2)
                .Select(item => new
                {
                    Flag = item.flag,
                    CompletedSize = completedSize += item.size,
                    TotalSize = totalSize,
                    RemotePath = remotePath,
                    LocalPath = localPath
                });

            // --------------------------------------------------------------------------------------------

            long previousCompletedSize = 0;
            var previousDateTime = DateTimeOffset.Now;
            const double period = 1000;

            task
                //.ObserveOn(Scheduler.CurrentThread)
                //.SubscribeOn(NewThreadScheduler.Default)
                .Timestamp()
                .Sample(TimeSpan.FromMilliseconds(period))
                .Subscribe(
                    timestampedContext =>
                    {
                        var timestamp = timestampedContext.Timestamp;
                        var context = timestampedContext.Value;

                        Console.WriteLine($"{context.Flag}, " +
                                          $"Id: {Thread.CurrentThread.ManagedThreadId:00}, " +
                                          $"{((context.CompletedSize - previousCompletedSize) / (timestamp - previousDateTime).TotalSeconds) / (1024 * 1024): 00.000} Mb/s, " +
                                          $"{100.0 * context.CompletedSize / context.TotalSize: 00.00}%, " +
                                          $"Delta Time: {(timestamp - previousDateTime).TotalSeconds}" /*+ $"Time: {DateTime.Now:O}"*/);

                        previousCompletedSize = context.CompletedSize;
                        previousDateTime = timestamp;
                    },
                    () =>
                    {
                        Console.WriteLine($"======= Completed! Time: {DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss} =======");
                    }
                );

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

        public static IObservable<(string flag, int size)> CopyStream(Task<Stream> outputStream, Stream inputStream, CancellationToken token)
        {
            return Observable.Create<(string flag, int size)>(async o =>
            {
                try
                {
                    byte[] buffer = new byte[128 * 1024];
                    int count;
                    var flag = $"BLOCK - {(inputStream.Position / BlockLength):000}";
                    Console.WriteLine($"Enter Id: {Thread.CurrentThread.ManagedThreadId}");
                    while ((count = await (await outputStream).ReadAsync(buffer, 0, buffer.Length, token)) > 0)
                    {
                        if (token.IsCancellationRequested) break;
                        await inputStream.WriteAsync(buffer, 0, count, token);
                        o.OnNext((flag, count));
                    }
                    Console.WriteLine($"{flag} Completed Id: {Thread.CurrentThread.ManagedThreadId} {inputStream.Position}");
                    o.OnCompleted();
                }
                catch (Exception e)
                {
                    o.OnError(e);
                }

                return () =>
                {
                    inputStream.Dispose();
                    outputStream.Dispose();
                };
            });
        }

        public static IEnumerable<(long Offset, long Length)> Split(long totalLength, long blockLength)
        {
            long offset = 0;
            while (offset + blockLength < totalLength)
            {
                yield return (offset, blockLength);
                offset += blockLength;
            }

            yield return (offset, totalLength - offset);
        }

        public static void AddRangeBasedOffsetLength(this HttpWebRequest @this, long offset, long length)
        {
            @this.AddRange(offset, offset + length - 1);
        }
    }
}

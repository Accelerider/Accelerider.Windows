using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Threading;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    public static class FileTransferService
    {
        public const long BlockLength = 1024 * 1024 * 50;

        public static HttpWebRequest CreateRequest(string remotePath, ITransferContext context)
        {
            ((TransferContext)context).RemotePath = remotePath;
            return WebRequest.CreateHttp(remotePath);
        }

        public static HttpWebResponse GetResponse(HttpWebRequest request, ITransferContext context)
        {
            var response = (HttpWebResponse)request.GetResponse();
            ((TransferContext)context).Response = response;
            ((TransferContext)context).TotalSize = response.ContentLength;
            return response;
        }

        public static HttpWebRequest ConfigureRequestDefault(HttpWebRequest request, ITransferContext context)
        {
            request.Headers = new WebHeaderCollection();
            request.Method = "GET";
            request.AddRange(100, 300);
            return request;
        }

        public static Stream GetStream(HttpWebResponse response, ITransferContext context)
        {
            return response.GetResponseStream();
        }

        public static Stream GetStream(string localPath, ITransferContext context)
        {
            ((TransferContext)context).LocalPath = localPath;
            return File.Open(localPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
        }

        public static IEnumerable<HttpWebRequest> GetBlockRequests(HttpWebResponse response, ITransferContext context)
        {
            if (string.IsNullOrEmpty(context.RemotePath))
                throw new InvalidOperationException();

            return Split(response.ContentLength, BlockLength).Select(item =>
            {
                var result = WebRequest.CreateHttp(context.RemotePath);
                result.AddRange(item.From, item.To);
                return result;
            });
        }

        public static Func<(string remotePath, string localPath), ITransferTask> GetDownloader(
            Func<string, ITransferContext, Stream> getRemoteStream,
            Func<string, ITransferContext, Stream> getLocalStream,
            ITransferContext context)
        {
            return pathPair =>
            {
                var outputStream = getRemoteStream(pathPair.remotePath, context);
                var inputStream = getLocalStream(pathPair.localPath, context);
                return CopyStream(outputStream, inputStream, (TransferContext)context);
            };
        }

        //private static Func<(string remotePath, string localPath), ITransferTask> GetDownloader(
        //    Func<string, ITransferContext, IEnumerable<Stream>> getRemoteStreams,
        //    Func<string, ITransferContext, Stream> getLocalStream,
        //    ITransferContext context)
        //{
        //    return pathPair =>
        //    {
        //        var outputStreams = getRemoteStreams(pathPair.remotePath, context);
        //        var inputStream = getLocalStream(pathPair.localPath, context);

        //    }
        //}

        //private static void DispatchStream()

        public static ITransferTask CopyStream(Stream outputStream, Stream inputStream, TransferContext context)
        {
            return new TransferTask(Observable.Create<ITransferContext>(async o =>
            {
                try
                {
                    Console.WriteLine($"Enter Id: {Thread.CurrentThread.ManagedThreadId}");
                    byte[] buffer = new byte[128 * 1024];
                    int count;
                    while ((count = await outputStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await inputStream.WriteAsync(buffer, 0, count);
                        context.CompletedSize += count;
                        o.OnNext(context);
                    }
                    o.OnCompleted();
                }
                catch (Exception e)
                {
                    o.OnError(e);
                }
                return () =>
                {
                    inputStream.Dispose();
                    context.Response.Dispose();
                    outputStream.Dispose();
                };
            }));
        }

        public static IEnumerable<(long From, long To)> Split(long totalLength, long blockLength)
        {
            long from = 0;
            while (from + blockLength < totalLength)
            {
                yield return (from, from + blockLength);
                from += blockLength;
            }

            yield return (from, totalLength);
        }

    }
}

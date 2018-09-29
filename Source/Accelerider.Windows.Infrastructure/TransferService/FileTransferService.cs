using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Polly;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    public static class FileTransferService
    {
        static FileTransferService()
        {
            ServicePointManager.DefaultConnectionLimit = 10000;
        }

        public static IDownloaderBuilder GetFileDownloaderBuilder()
        {
            return new FileDownloaderBuilder();
        }

        public static IDownloaderBuilder UseDefaultConfigure(this IDownloaderBuilder @this)
        {
            return @this
                .Configure(request =>
                {
                    request.Headers.SetHeaders(new WebHeaderCollection { ["User-Agent"] = "Accelerider.Windows.DownloadEngine" });
                    request.Method = "GET";
                    request.Timeout = 1000 * 30;
                    request.ReadWriteTimeout = 1000 * 30;
                    return request;
                })
                .Configure(localPath =>
                {
                    var directoryName = Path.GetDirectoryName(localPath) ?? throw new InvalidOperationException();
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(localPath) ?? throw new InvalidOperationException();
                    var extension = Path.GetExtension(localPath);
                    for (int i = 1; File.Exists(localPath); i++)
                    {
                        localPath = $"{Path.Combine(directoryName, fileNameWithoutExtension)} ({i}){extension}";
                    }

                    return localPath;
                })
                .Configure(DefaultBlockIntervalGenerator)
                .Configure((settings, context) =>
                {
                    settings.MaxConcurrent = 16;

                    settings.BuildPolicy = Policy
                        .Handle<WebException>()
                        .RetryAsync(context.RemotePathProvider.RemotePaths.Count, (e, retryCount, policyContext) =>
                        {
                            var remotePath = ((WebException)e).Response.ResponseUri.OriginalString;
                            context.RemotePathProvider.Vote(remotePath, -3);
                        });

                    settings.DownloadPolicy
                        .Catch<OperationCanceledException>((e, retryCount, blockContext) => HandleCommand.Break)
                        .Catch<WebException>((e, retryCount, blockContext) => retryCount < 3 ? HandleCommand.Retry : HandleCommand.Throw);
                });
        }

        private static IEnumerable<(long Offset, long Length)> DefaultBlockIntervalGenerator(long totalLength)
        {
            const long blockLength = 1024 * 1024 * 20;

            long offset = 0;
            while (offset + blockLength < totalLength)
            {
                yield return (offset, blockLength);
                offset += blockLength;
            }

            yield return (offset, totalLength - offset);
        }
    }
}

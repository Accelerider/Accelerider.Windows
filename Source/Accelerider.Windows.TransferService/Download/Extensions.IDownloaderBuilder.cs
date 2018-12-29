using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Polly;

namespace Accelerider.Windows.TransferService
{
    public static class DownloaderBuilderExtensions
    {
        private const int WebExceptionRetryCount = 20;
        private const int BuildExceptionRetryCount = 5;

        public static IDownloaderBuilder UseDefaultConfigure(this IDownloaderBuilder @this) => @this
            .Configure(request =>
            {
                request.Headers.SetHeaders(new WebHeaderCollection { ["User-Agent"] = "Accelerider.Windows.DownloadEngine" });
                request.Method = "GET";
                request.Timeout = 1000 * 30;
                request.ReadWriteTimeout = 1000 * 30;
                return request;
            })
            .Configure(localPath => localPath.GetUniqueLocalPath())
            .Configure(DefaultBlockIntervalGenerator)
            .Configure((settings, context) =>
            {
                settings.MaxConcurrent = 16;

                settings.BuildPolicy = Policy
                    .Handle<WebException>(e => e.Status != WebExceptionStatus.RequestCanceled)
                    .RetryAsync(BuildExceptionRetryCount, (e, retryCount, policyContext) =>
                    {
                        var remotePath = ((WebException)e).Response?.ResponseUri.OriginalString;
                        if (!string.IsNullOrEmpty(remotePath))
                            context.RemotePathProvider.Rate(remotePath, -1);
                    });

                settings.DownloadPolicy
                    .Catch<OperationCanceledException>((e, retryCount, blockContext) => HandleCommand.Break)
                    .Catch<WebException>((e, retryCount, blockContext) => retryCount < WebExceptionRetryCount ? HandleCommand.Retry : HandleCommand.Throw)
                    .Catch<RemotePathExhaustedException>((e, retryCount, blockContext) => HandleCommand.Throw);
            });

        private static IEnumerable<(long Offset, long Length)> DefaultBlockIntervalGenerator(long totalLength)
        {
            const long defaultBlockLength = 1024 * 1024 * 20; // 20MB
            const int maxBlockCount = 100;

            var preCount = totalLength / defaultBlockLength;

            var blockLength = preCount > maxBlockCount ? (totalLength / (maxBlockCount - 1)) : defaultBlockLength;

            long offset = 0;
            while (offset + blockLength < totalLength)
            {
                yield return (offset, blockLength);
                offset += blockLength;
            }

            yield return (offset, totalLength - offset);
        }

        public static IDownloaderBuilder From(this IDownloaderBuilder @this, string path) => @this.From(new[] { path });

        public static IDownloaderBuilder From(this IDownloaderBuilder @this, IEnumerable<string> paths)
        {
            return @this.From(new ConstantRemotePathProvider(new HashSet<string>(paths)));
        }

        public static IDownloaderBuilder From(this IDownloaderBuilder @this, Func<Task<string>> remotePathGetter)
        {
            return @this.From(new AsyncRemotePathProvider(remotePathGetter));
        }

        public static IDownloader Build(this IDownloaderBuilder @this, string jsonText)
        {
            var data = jsonText.ToObject<DownloadSerializedData>();

            data.Context.RemotePathProvider = data.RemotePathProviderPersister.Restore();

            return @this.Build(data.Context, data.BlockContexts);
        }
    }
}

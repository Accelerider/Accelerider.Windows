using Polly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    public static partial class FileTransferService
    {
        public const string DefaultConfigureTag = "DefaultConfigure";
        public const string BaiduCloudConfigureTag = "BaiduCloudConfigure";
        public const string OneDriveConfigureTag = "OneDriveConfigure";
        public const string OneOneFiveCloudConfigureTag = "OneOneFiveCloudConfigure";
        public const string SixCloudConfigureTag = "SixCloudConfigure";

        public static IDownloaderBuilder UseConfigure(this IDownloaderBuilder @this, string configureTag)
        {
            switch (configureTag)
            {
                case BaiduCloudConfigureTag:
                    return @this.UseBaiduCloudConfigure();
                case OneDriveConfigureTag:
                    return @this.UseOneDriveConfigure();
                case OneOneFiveCloudConfigureTag:
                    return @this.UseOneOneFiveCloudConfigure();
                case SixCloudConfigureTag:
                    return @this.UseSixCloudConfigure();
                default:
                    return @this.UseDefaultConfigure();
            }
        }

        public static IDownloaderBuilder UseDefaultConfigure(this IDownloaderBuilder @this) => @this
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
                        .Catch<WebException>((e, retryCount, blockContext) => retryCount < 3 ? HandleCommand.Retry : HandleCommand.Throw)
                        .Catch<RemotePathExhaustedException>((e, retryCount, blockContext) => HandleCommand.Throw);
                })
            .Configure(downloader =>
            {
                downloader.Tag = DefaultConfigureTag;
                return downloader;
            });

        public static IDownloaderBuilder UseBaiduCloudConfigure(this IDownloaderBuilder @this) => @this
            .Configure(downloader =>
            {
                downloader.Tag = BaiduCloudConfigureTag;
                return downloader;
            });

        public static IDownloaderBuilder UseOneDriveConfigure(this IDownloaderBuilder @this) => @this
            .Configure(downloader =>
            {
                downloader.Tag = OneDriveConfigureTag;
                return downloader;
            });

        public static IDownloaderBuilder UseOneOneFiveCloudConfigure(this IDownloaderBuilder @this) => @this
            .Configure(downloader =>
            {
                downloader.Tag = OneOneFiveCloudConfigureTag;
                return downloader;
            });

        public static IDownloaderBuilder UseSixCloudConfigure(this IDownloaderBuilder @this) => @this
            .Configure(downloader =>
            {
                downloader.Tag = SixCloudConfigureTag;
                return downloader;
            });

        private static IEnumerable<(long Offset, long Length)> DefaultBlockIntervalGenerator(long totalLength)
        {
            const long defaultBlockLength = 1024 * 1024 * 20;

            var preCount = totalLength / defaultBlockLength;

            var blockLength = preCount > 100 ? (totalLength / 99) : defaultBlockLength;

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

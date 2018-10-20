using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Accelerider.Windows.TransferService;
using Newtonsoft.Json;

namespace Accelerider.Windows.Infrastructure.Modularity
{
    public class AppMetadata
    {
        [JsonProperty]
        public string Id { get; private set; }

        [JsonProperty]
        public string Name { get; private set; }

        [JsonProperty]
        public Version Version { get; private set; }

        [JsonProperty]
        public List<AppFileMetadata> Files { get; private set; }
    }

    public class AppFileMetadata
    {
        [JsonProperty]
        public string Name { get; private set; }

        [JsonProperty]
        public string Checksum { get; private set; }

        [JsonProperty]
        public RemoteRef Ref { get; private set; }
    }

    public static class AppMetadataExtensions
    {
        private static readonly MD5 Md5Algorithm = MD5.Create();




        public static IEnumerable<AppMetadata> GetMissingOrNonLastestApps(this IReadOnlyCollection<AppMetadata> requiredApps, IReadOnlyCollection<AppMetadata> complementarySet)
        {
            var missingApps = requiredApps.Where(item => !Directory.Exists(item.GetDirectoryPath()));

            var nonLastestApps = (
                from app in requiredApps
                let directory = app.GetDirectoryPath()
                where Directory.Exists(directory)
                where directory.GetVersionFromPath() < app.Version
                select app
            );

            return missingApps.Union(nonLastestApps);
        }

        public static void Activate(this IEnumerable<IDownloader> @this)
        {
            @this.ForEach(item => item.Run());
        }

        public static IEnumerable<IDownloader> ToDownloaders(this IEnumerable<RemoteRef> @this)
        {
            return @this.Select(item => FileTransferService
                .GetDownloaderBuilder()
                .UseDefaultConfigure()
                .From(item.RemotePath)
                .To(item.LocalPath)
                .Build());
        }

        private static string GetFileMd5(this string filePath)
        {
            Guards.ThrowIfFileNotFound(filePath);

            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return BitConverter.ToString(Md5Algorithm.ComputeHash(fileStream)).ToLower().Replace("-", string.Empty);
            }
        }

        private static AppFileMetadata InitializeLocalPath(this AppFileMetadata @this)
        {
            @this.Ref.LocalPath = @this.Ref.LocalPath
                .Replace("{AppsFolder}", AcceleriderPaths.AppsFolder);
            return @this;
        }

        private static string GetDirectoryPath(this AppMetadata @this)
        {
            return Path.Combine(AcceleriderPaths.AppsFolder, @this.Name, $"bin-{@this.Version.ToString(3)}");
        }

        private static readonly Regex GetVersionRegex = new Regex(@"bin-(\d+?\.\d+?\.\d+?)$", RegexOptions.Compiled);

        private static Version GetVersionFromPath(this string path)
        {
            var match = GetVersionRegex.Match(path);
            if (match.Success)
            {
                return new Version(match.Groups[1].Value);
            }

            throw new ArgumentException("The path is not a valid app path. ", nameof(path));
        }
    }
}

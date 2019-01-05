using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Accelerider.Windows.TransferService;

namespace Accelerider.Windows.Infrastructure.Upgrade
{
    public abstract class UpgradeTaskBase : IUpgradeTask
    {
        private readonly Regex VersionRegex;
        private readonly string _folderPrefix;
        private Version _latestVersion;

        public string Name { get; }

        public string InstallDirectory { get; }

        public Version LatestVersion
        {
            get => _latestVersion ?? (_latestVersion = GetCurrentVersion());
            set => _latestVersion = value;
        }

        protected UpgradeTaskBase(string name, string installDirectory, string folderPrefix = null)
        {
            Name = name;
            InstallDirectory = installDirectory;
            _folderPrefix = folderPrefix ?? name;
            VersionRegex = new Regex($@"^{name}-(\d+?\.\d+?\.\d+?)$", RegexOptions.Compiled);
        }

        public async Task ExecuteAsync(UpgradeInfo info)
        {
            if (!PrepareUpgrade(info))
            {
                OnCompleted(info, false);
                return;
            }

            try
            {
                await ResolveFileAsync(info);

                LatestVersion = info.Version;
                OnCompleted(info, true);
            }
            catch (Exception e)
            {
                OnError(e);
            }
        }

        protected virtual bool PrepareUpgrade(UpgradeInfo info)
        {
            return info.Version > GetCurrentVersion();
        }

        public virtual Version GetCurrentVersion()
        {
            var versions = Directory.GetDirectories(InstallDirectory)
                .Select(Path.GetFileName)
                .Select(item =>
                {
                    var match = VersionRegex.Match(item);
                    return match.Success ? match.Groups[1].Value : null;
                })
                .Where(item => item != null)
                .Select(Version.Parse)
                .ToArray();

            return versions.Any() ? versions.Max() : new Version();
        }

        protected virtual void OnCompleted(UpgradeInfo info, bool upgraded) { }

        protected virtual void OnError(Exception e) { }

        private async Task ResolveFileAsync(UpgradeInfo info)
        {
            using (var tempPath = new TempDirectory(Path.Combine(Path.GetTempPath(), $"{Name}-{Path.GetRandomFileName()}")))
            {
                var zipFilePath = Path.Combine(tempPath.DirectoryPath, Path.GetRandomFileName());

                // 1. Download the module
                var downloader = FileTransferService
                    .GetDownloaderBuilder()
                    .UseDefaultConfigure()
                    .From(info.Url)
                    .To(zipFilePath)
                    .Build();

                downloader.Run();

                await downloader;

                // 2. Unzip the module
                ZipFile.ExtractToDirectory(zipFilePath, tempPath.DirectoryPath);

                // 3. Move file to target path.
                var targetPath = Path.Combine(InstallDirectory, $"{_folderPrefix}-{info.Version.ToString(3)}");
                tempPath.MoveTo(targetPath, Directory.GetDirectories(tempPath.DirectoryPath, $"{Name}-*").FirstOrDefault());
            }
        }
    }
}

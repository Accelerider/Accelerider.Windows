using System;
using System.Collections.Generic;
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
        protected static readonly Version EmptyVersion = new Version();

        private readonly Regex VersionRegex;
        private readonly string _folderPrefix;

        public string Name { get; }

        public string InstallDirectory { get; }

        public Version CurrentVersion { get; private set; } = EmptyVersion;

        protected UpgradeTaskBase(string name, string installDirectory, string folderPrefix = null)
        {
            Name = name;
            InstallDirectory = installDirectory;
            _folderPrefix = folderPrefix ?? name;
            VersionRegex = new Regex($@"^{_folderPrefix}-(\d+?\.\d+?\.\d+?)$", RegexOptions.Compiled);
        }

        public async Task ExecuteAsync(UpgradeInfo info)
        {
            CurrentVersion = GetMaxLocalVersion();
            if (CurrentVersion > EmptyVersion)
            {
                OnCompleted(info, false);
            }

            if (!PrepareUpgrade(info)) return;

            try
            {
                await ResolveFileAsync(info);

                OnCompleted(info, true);
                CurrentVersion = GetMaxLocalVersion();
            }
            catch (Exception e)
            {
                OnError(e);
            }
        }

        protected virtual bool PrepareUpgrade(UpgradeInfo info)
        {
            return info.Version > GetMaxLocalVersion();
        }

        public virtual Version GetMaxLocalVersion()
        {
            var versions = GetLocalVersions()
                .Select(item => item.Version)
                .ToArray();

            return versions.Any() ? versions.Max() : EmptyVersion;
        }

        protected virtual IEnumerable<(Version Version, string Path)> GetLocalVersions()
        {
            return from folderPath in Directory.GetDirectories(InstallDirectory)
                   let folderName = Path.GetFileName(folderPath)
                   where !string.IsNullOrEmpty(folderName)
                   let match = VersionRegex.Match(folderName)
                   where match.Success
                   select (Version.Parse(match.Groups[1].Value), folderPath);
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

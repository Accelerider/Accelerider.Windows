using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Accelerider.Windows.TransferService;
using log4net;

namespace Accelerider.Windows.Infrastructure.Upgrade
{
    public abstract class UpgradeTaskBase : IUpgradeTask
    {
        protected static readonly Version EmptyVersion = new Version("0.0.0.0");

        private readonly Regex _versionRegex;
        private readonly string _folderPrefix;

        public string Name { get; }

        public string InstallDirectory { get; }

        public Version CurrentVersion { get; private set; } = EmptyVersion;

        protected ILog Logger { get; }

        protected UpgradeTaskBase(string name, string installDirectory, string folderPrefix = null)
        {
            Logger = LogManager.GetLogger(GetType());

            Name = name;
            InstallDirectory = installDirectory;
            _folderPrefix = folderPrefix ?? name;
            _versionRegex = new Regex($@"^{_folderPrefix}-(\d+?\.\d+?\.\d+?)$", RegexOptions.Compiled);
        }

        public async Task LoadFromRemoteAsync(UpgradeInfo info)
        {
            // current-version, max-local-version, upgrade-info-version

            // 1. upgrade-info-version > current-version --> required upgrade
            if (!IsUpgradeRequired(info)) return;

            try
            {
                // 2. upgrade-info-version > max-local-version --> required download
                if (IsDownloadRequired(info)) await ResolveFileAsync(info);

                OnDownloadCompleted(info);
            }
            catch (Exception e)
            {
                OnDownloadError(e);
            }
        }

        public abstract Task LoadFromLocalAsync();

        protected virtual bool IsUpgradeRequired(UpgradeInfo info)
        {
            if (CurrentVersion == EmptyVersion) CurrentVersion = GetMaxLocalVersion();

            return info.Version > CurrentVersion;
        }

        protected virtual bool IsDownloadRequired(UpgradeInfo info)
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
                   let match = _versionRegex.Match(folderName)
                   where match.Success
                   select (Version.Parse(match.Groups[1].Value), folderPath);
        }

        protected virtual void OnDownloadCompleted(UpgradeInfo info)
        {
            Logger.Info($"[Upgrade] The {info.Name}-{info.Version.ToString(3)} has been downloaded. ");
        }

        protected virtual void OnDownloadError(Exception e)
        {
            Logger.Error("An unexpected exception occured when downloading module. ", e);
        }

        private async Task ResolveFileAsync(UpgradeInfo info)
        {
            using (var tempPath = new TempDirectory(Path.Combine(Path.GetTempPath(), $"{Name}-{Path.GetRandomFileName()}")))
            {
                var zipFilePath = Path.Combine(tempPath.Path, Path.GetRandomFileName());

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
                ZipFile.ExtractToDirectory(zipFilePath, tempPath.Path);

                // 3. Move file to target path.
                tempPath.MoveTo(GetInstallPath(info.Version),
                    Directory.GetDirectories(tempPath.Path).FirstOrDefault());
            }
        }

        protected string GetInstallPath(Version version) =>
            Path.Combine(InstallDirectory, $"{_folderPrefix}-{version.ToString(3)}");
    }
}

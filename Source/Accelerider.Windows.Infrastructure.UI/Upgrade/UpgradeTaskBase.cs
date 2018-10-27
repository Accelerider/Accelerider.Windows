using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.JsonObjects;

namespace Accelerider.Windows.Infrastructure.Upgrade
{
    public abstract class UpgradeTaskBase : IUpgradeTask
    {
        private static readonly Regex VersionRegex = new Regex(@"^bin-(\d+?-\d+?-\d+?)$", RegexOptions.Compiled);
        private readonly string InstallDirectory;

        protected UpgradeTaskBase(string name)
        {
            Name = name;
            InstallDirectory = Path.Combine(AcceleriderPaths.AppsFolder, name);
            Directory.CreateDirectory(InstallDirectory);
        }

        public Version CurrentVersion { get; protected set; } = new Version();

        public string Name { get; }

        public async Task<bool> UpdateAsync(AppMetadata metadata)
        {
            PrepareUpgrade();

            if (!RequireUpgrade(metadata, out var latestVersion)) return true;

            var upgradeInfo = GetUpgradeInfo(latestVersion);
            var upgradeResult = await UpgradeCoreAsync(upgradeInfo);

            if (upgradeResult)
            {
                CurrentVersion = upgradeInfo.Version;
            }

            return upgradeResult;
        }

        protected virtual void PrepareUpgrade()
        {
            CurrentVersion = GetMaxLocalVersion();
        }

        protected virtual bool RequireUpgrade(AppMetadata metadata, out Version latestVersion)
        {
            latestVersion = IsExperimentalEnabled(metadata.LatestVersion.ExperimentalPercentage)
                ? metadata.LatestVersion.ExperimentalVersion
                : metadata.LatestVersion.StableVersion;

            return latestVersion > CurrentVersion;
        }

        protected virtual bool IsExperimentalEnabled(double experimentalPercentage) => false;

        protected abstract Task<bool> UpgradeCoreAsync(UpgradeInfo metadata);

        private Version GetMaxLocalVersion()
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

        private UpgradeInfo GetUpgradeInfo(Version version)
        {
            var appFileList = GetAppFileList(version);

            var privateFiles = appFileList.PrivateFiles.Select(item => (item.Name, GetFileDownloadUrl(version, item.Name))).ToList();
            var publicFiles = appFileList.PublicFiles.Select(item => (item.Name, GetFileDownloadUrl(version, item.Name))).ToList();

            return new UpgradeInfo(version, privateFiles, publicFiles);
        }

        private AppFileList GetAppFileList(Version version)
        {
            throw new NotImplementedException();
        }

        private string GetFileDownloadUrl(Version version, string fileName)
        {
            return $"https://api.accelerider.com/v2/apps/{Name}/{version.ToString(3)}/{fileName}/content";
        }
    }
}

using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.JsonObjects;
using Accelerider.Windows.TransferService;

namespace Accelerider.Windows.Infrastructure.Upgrade
{
    public abstract class UpgradeTaskBase : IUpgradeTask
    {
        protected UpgradeTaskBase(string name)
        {
            Name = name;
        }

        public Version CurrentVersion { get; protected set; } = new Version();

        public string Name { get; }

        public IDownloader Downloader { get; private set; }

        public async Task UpdateAsync(AppMetadata metadata)
        {
            PrepareUpgrade();

            if (RequireUpgrade(metadata))
            {
                Downloader = UpgradeCoreAsync(metadata);
                Downloader.Run();
                await Downloader;
                if (Downloader.Status == TransferStatus.Completed)
                    CurrentVersion = metadata.Version.StableVersion;
            }
        }

        protected virtual void PrepareUpgrade() { }

        protected virtual bool RequireUpgrade(AppMetadata metadata)
        {
            var latestVersion = IsExperimentalEnabled(metadata.Version.ExperimentalPercentage)
                ? metadata.Version.ExperimentalVersion
                : metadata.Version.StableVersion;

            return latestVersion > CurrentVersion;
        }

        protected virtual bool IsExperimentalEnabled(double experimentalPercentage) => false;

        protected abstract IDownloader UpgradeCoreAsync(AppMetadata metadata);
    }
}

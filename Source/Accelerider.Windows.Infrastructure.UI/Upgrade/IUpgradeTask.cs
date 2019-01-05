using System;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.Upgrade
{
    public interface IUpgradeTask
    {
        Version LatestVersion { get; }

        string Name { get; }

        Version GetCurrentVersion();

        Task ExecuteAsync(UpgradeInfo info);
    }
}

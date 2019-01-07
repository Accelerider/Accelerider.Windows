using System;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.Upgrade
{
    public interface IUpgradeTask
    {
        Version CurrentVersion { get; }

        string Name { get; }

        Task ExecuteAsync(UpgradeInfo info);
    }
}

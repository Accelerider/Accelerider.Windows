using System;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.JsonObjects;
using Accelerider.Windows.TransferService;

namespace Accelerider.Windows.Infrastructure.Upgrade
{
    public interface IUpgradeTask
    {
        Version CurrentVersion { get; }

        string Name { get; }

        IDownloader Downloader { get; }

        Task UpdateAsync(AppMetadata metadata);
    }
}

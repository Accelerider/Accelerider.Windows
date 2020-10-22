using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.Upgrade
{
    public interface IUpgradeTask
    {
        string Name { get; }

        Task LoadFromLocalAsync();

        Task LoadFromRemoteAsync(UpgradeInfo info);
    }
}

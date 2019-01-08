using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.Upgrade
{
    public interface IUpgradeTask
    {
        string Name { get; }

        Task DownloadAsync(UpgradeInfo info);

        Task<bool> TryLoadAsync();
    }
}

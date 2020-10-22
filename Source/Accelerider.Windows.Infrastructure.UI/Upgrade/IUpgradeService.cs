namespace Accelerider.Windows.Infrastructure.Upgrade
{
    public interface IUpgradeService
    {
        void Run();

        void Stop();

        void Add(IUpgradeTask task);

        IUpgradeTask Get(string name);
    }
}

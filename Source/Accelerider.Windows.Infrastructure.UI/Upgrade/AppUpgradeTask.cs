using System;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.Upgrade
{
    public class AppUpgradeTask : UpgradeTaskBase
    {
        public AppUpgradeTask(string name) : base(name) { }

        protected override Task<bool> UpgradeCoreAsync(UpgradeInfo metadata)
        {
            throw new NotImplementedException();
        }
    }
}

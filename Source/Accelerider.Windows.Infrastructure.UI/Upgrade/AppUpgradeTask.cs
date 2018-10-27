using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.TransferService;

namespace Accelerider.Windows.Infrastructure.Upgrade
{
    public class AppUpgradeTask : UpgradeTaskBase
    {
        public AppUpgradeTask(string name) : base(name) { }

        protected override Task<bool> UpgradeCoreAsync(UpgradeInfo metadata)
        {
        }
    }
}

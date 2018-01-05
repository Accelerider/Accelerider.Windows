using Accelerider.Windows.Infrastructure;
using Microsoft.Practices.Unity;
using Prism.Regions;

namespace Accelerider.Windows.Modules.NetDisk.Plus
{
    public class NetDiskModule : ModuleBase
    {
        private readonly IRegionManager _regionManager;

        public NetDiskModule(IUnityContainer container, IRegionManager regionManager) : base(container)
        {
            _regionManager = regionManager;
        }

        public override void Initialize()
        {
        }
    }
}

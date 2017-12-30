using Accelerider.Windows.Infrastructure;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;

namespace Accelerider.Windows.Modules.NetDisk
{
    public class NetDiskModule : IModule
    {
        IRegionManager _regionManager;
        IUnityContainer _container;

        public NetDiskModule(RegionManager regionManager, IUnityContainer container)
        {
            _regionManager = regionManager;
            _container = container;
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion(RegionNames.MainTabRegion, typeof());
            _regionManager.RegisterViewWithRegion(RegionNames.MainTabRegion, typeof());
        }
    }
}

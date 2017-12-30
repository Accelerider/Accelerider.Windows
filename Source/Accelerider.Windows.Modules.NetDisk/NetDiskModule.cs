using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Modules.NetDisk.Views.FileBrowser;
using Accelerider.Windows.Modules.NetDisk.Views.Transmission;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;

namespace Accelerider.Windows.Modules.NetDisk
{
    public class NetDiskModule : IModule
    {
        private readonly IRegionManager _regionManager;
        private readonly IUnityContainer _container;

        public NetDiskModule(RegionManager regionManager, IUnityContainer container)
        {
            _regionManager = regionManager;
            _container = container;
        }

        public void Initialize()
        {
            _container.Resolve<Components.Authenticator.Module>().Initialize();

            _regionManager.RegisterViewWithRegion(RegionNames.MainTabRegion, typeof(FileBrowserComposite));
            _regionManager.RegisterViewWithRegion(RegionNames.MainTabRegion, typeof(TransmissionComposite));
        }
    }
}

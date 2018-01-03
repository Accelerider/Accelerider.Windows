using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Modules.NetDisk.Views;
using Accelerider.Windows.Modules.NetDisk.Views.FileBrowser;
using Accelerider.Windows.Modules.NetDisk.Views.Transmission;
using Microsoft.Practices.Unity;
using Prism.Regions;

namespace Accelerider.Windows.Modules.NetDisk
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
            _regionManager.RegisterViewWithRegion(RegionNames.MainTabRegion, typeof(FileBrowserComponent));
            _regionManager.RegisterViewWithRegion(RegionNames.MainTabRegion, typeof(TransmissionComponent));

            _regionManager.RegisterViewWithRegion(RegionNames.SettingsTabRegion, typeof(TaskSettingsTabItem));
        }
    }
}

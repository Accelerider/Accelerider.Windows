using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Modularity;
using Accelerider.Windows.Modules.NetDisk.Views;
using Accelerider.Windows.Modules.NetDisk.Views.FileBrowser;
using Accelerider.Windows.Modules.NetDisk.Views.Transportation;
using Accelerider.Windows.TransferService;
using Prism.Events;
using Prism.Ioc;
using Unity;
using Prism.Regions;
using Prism.Unity;

namespace Accelerider.Windows.Modules.NetDisk
{
    public class NetDiskModule : ModuleBase
    {
        private readonly IRegionManager _regionManager;

        public NetDiskModule(IUnityContainer container, IRegionManager regionManager) : base(container)
        {
            _regionManager = regionManager;
        }

        public override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register for container
            containerRegistry.RegisterInstance(FileTransferService.GetDownloaderManager("net-disk"));

            AcceleriderUserExtensions.Initialize(containerRegistry.GetContainer());

            // Register for region
            _regionManager.RegisterViewWithRegion(RegionNames.MainTabRegion, typeof(FileBrowserComponent));
            _regionManager.RegisterViewWithRegion(RegionNames.MainTabRegion, typeof(TransportationComponent));
            _regionManager.RegisterViewWithRegion(RegionNames.SettingsTabRegion, typeof(TaskSettingsTabItem));
        }

        public override void OnInitialized(IContainerProvider containerProvider)
        {
            containerProvider
                .Resolve<IEventAggregator>()
                .GetEvent<ApplicationExiting>()
                .Subscribe(
                    () =>
                    {
                        if (containerProvider.GetContainer().IsRegistered<IAcceleriderUser>())
                        {
                            containerProvider.Resolve<IAcceleriderUser>().SaveToLocalDisk();
                        }
                    },
                    true);
        }
    }
}
   
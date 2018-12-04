using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Modularity;
using Accelerider.Windows.Modules.NetDisk.Constants;
using Accelerider.Windows.Modules.NetDisk.Interfaces;
using Accelerider.Windows.Modules.NetDisk.Views;
using Accelerider.Windows.Modules.NetDisk.Views.FileBrowser;
using Accelerider.Windows.Modules.NetDisk.Views.Transportation;
using Accelerider.Windows.TransferService;
using Prism.Ioc;
using Unity;
using Prism.Regions;
using Refit;

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
            containerRegistry.RegisterInstance(GetNetDiskApi());
            containerRegistry.RegisterInstance(FileTransferService.GetDownloaderManager("net-disk"));

            // Register for region
            _regionManager.RegisterViewWithRegion(RegionNames.MainTabRegion, typeof(FileBrowserComponent));
            _regionManager.RegisterViewWithRegion(RegionNames.MainTabRegion, typeof(TransportationComponent));
            _regionManager.RegisterViewWithRegion(RegionNames.SettingsTabRegion, typeof(TaskSettingsTabItem));
        }

        public override void OnInitialized(IContainerProvider containerProvider)
        {
            AcceleriderUserExtensions.Initialize(Container);
        }

        private INetDiskApi GetNetDiskApi()
        {
            return RestService.For<INetDiskApi>(Hyperlinks.ApiBaseAddress, new RefitSettings
            {
                AuthorizationHeaderValueGetter = () => Task.FromResult(Container.Resolve<IAcceleriderUser>().Token)
            });
        }
    }
}

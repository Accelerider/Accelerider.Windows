using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Modularity;
using Accelerider.Windows.Modules.NetDisk.Constants;
using Accelerider.Windows.Modules.NetDisk.Extensions;
using Accelerider.Windows.Modules.NetDisk.Interfaces;
using Accelerider.Windows.Modules.NetDisk.Views;
using Accelerider.Windows.Modules.NetDisk.Views.FileBrowser;
using Accelerider.Windows.Modules.NetDisk.Views.Transportation;
using Autofac;
using Prism.Regions;
using Refit;

namespace Accelerider.Windows.Modules.NetDisk
{
    public class NetDiskModule : ModuleBase
    {
        private readonly IRegionManager _regionManager;

        public NetDiskModule(IContainer container, IRegionManager regionManager) : base(container)
        {
            _regionManager = regionManager;
        }

        public override void Initialize()
        {
            base.Initialize();

            _regionManager.RegisterViewWithRegion(RegionNames.MainTabRegion, typeof(FileBrowserComponent));
            _regionManager.RegisterViewWithRegion(RegionNames.MainTabRegion, typeof(TransportationComponent));
            _regionManager.RegisterViewWithRegion(RegionNames.SettingsTabRegion, typeof(TaskSettingsTabItem));

            AcceleriderUserExtensions.Initialize(Container);
        }

        protected override void ConfigureContainerBuilder(ContainerBuilder builder)
        {
            var acceleriderApi = RestService.For<INetDiskApi>(Hyperlinks.ApiBaseAddress, new RefitSettings
            {
                AuthorizationHeaderValueGetter = () => Task.FromResult(Container.Resolve<IAcceleriderUser>().Token)
                //AuthorizationHeaderValueGetter = () => Task.FromResult("")
            });
            builder.RegisterInstance(acceleriderApi).As<INetDiskApi>();
        }
    }
}

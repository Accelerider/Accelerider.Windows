using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Modularity;
using Accelerider.Windows.Modules.Group.Views;
using Prism.Ioc;
using Prism.Regions;
using Unity;

namespace Accelerider.Windows.Modules.Group
{
    public class GroupModule : ModuleBase
    {
        private readonly IRegionManager _regionManager;

        public GroupModule(IUnityContainer container, IRegionManager regionManager) : base(container)
        {
            _regionManager = regionManager;
        }

        public override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            _regionManager.RegisterViewWithRegion(RegionNames.MainTabRegion, typeof(GroupComponent));
            _regionManager.RegisterViewWithRegion(RegionNames.SettingsTabRegion, typeof(GroupSettingsTabItem));
        }
    }
}

using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Modularity;
using Accelerider.Windows.Modules.Group.Views;
using Autofac;
using Prism.Regions;

namespace Accelerider.Windows.Modules.Group
{
    public class GroupModule : ModuleBase
    {
        private readonly IRegionManager _regionManager;

        public GroupModule(IContainer container, IRegionManager regionManager) : base(container)
        {
            _regionManager = regionManager;
        }


        public override void Initialize()
        {
            _regionManager.RegisterViewWithRegion(RegionNames.MainTabRegion, typeof(GroupComponent));

            _regionManager.RegisterViewWithRegion(RegionNames.SettingsTabRegion, typeof(GroupSettingsTabItem));
        }
    }
}

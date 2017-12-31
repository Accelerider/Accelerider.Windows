using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Modules.Teams.Views;
using Microsoft.Practices.Unity;
using Prism.Regions;

namespace Accelerider.Windows.Modules.Teams
{
    public class TeamsModule : ModuleBase
    {
        private readonly IRegionManager _regionManager;

        public TeamsModule(IUnityContainer container, IRegionManager regionManager) : base(container)
        {
            _regionManager = regionManager;
        }


        public override void Initialize()
        {
            _regionManager.RegisterViewWithRegion(RegionNames.MainTabRegion, typeof(TeamsComposite));
        }
    }
}

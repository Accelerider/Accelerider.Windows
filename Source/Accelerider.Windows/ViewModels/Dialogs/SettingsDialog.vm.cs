using Accelerider.Windows.Infrastructure;
using Microsoft.Practices.Unity;
using Prism.Regions;

namespace Accelerider.Windows.ViewModels.Dialogs
{
    public class SettingsDialogViewModel : ViewModelBase
    {
        public SettingsDialogViewModel(IUnityContainer container, IRegionManager regionManager) : base(container)
        {
            RegionManager = regionManager;
        }

        public IRegionManager RegionManager { get; }
    }
}

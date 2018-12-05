using Accelerider.Windows.Infrastructure.Mvvm;
using Prism.Regions;
using Unity;

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

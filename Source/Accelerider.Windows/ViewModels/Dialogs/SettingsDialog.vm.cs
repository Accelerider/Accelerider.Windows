using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.ViewModels;
using Autofac;
using Prism.Regions;

namespace Accelerider.Windows.ViewModels.Dialogs
{
    public class SettingsDialogViewModel : ViewModelBase
    {
        public SettingsDialogViewModel(IContainer container, IRegionManager regionManager) : base(container)
        {
            RegionManager = regionManager;
        }

        public IRegionManager RegionManager { get; }
    }
}

using Accelerider.Windows.Common;
using Accelerider.Windows.Modules.NetDisk.ViewModels;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.ViewModels.Entering
{
    public class EnteringWindowViewModel : ViewModelBase
    {
        private bool _isLoading;


        public EnteringWindowViewModel(IUnityContainer container) : base(container)
        {
            EventAggregator.GetEvent<IsLoadingMainWindowEvent>().Subscribe(e => IsLoading = e);
        }


        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }
    }
}

using Accelerider.Windows.Modules.NetDisk.ViewModels.Others;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.Modules.NetDisk.ViewModels.Transmission
{
    public class DownloadingViewModel : TransferringBaseViewModel
    {
        public DownloadingViewModel(IUnityContainer container) : base(container)
        {
        }

        protected override TransferringTaskList GetTaskList() => Container.Resolve<TransferringTaskList>(TransferringTaskList.DownloadKey);
    }
}

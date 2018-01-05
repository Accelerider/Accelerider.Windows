using Accelerider.Windows.Modules.NetDisk.ViewModels.Others;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.Modules.NetDisk.ViewModels.Transportation
{
    public class UploadingViewModel : TransportingBaseViewModel
    {
        public UploadingViewModel(IUnityContainer container) : base(container)
        {
        }

        protected override TransferringTaskList GetTaskList() => Container.Resolve<TransferringTaskList>(TransferringTaskList.UploadKey);
    }
}

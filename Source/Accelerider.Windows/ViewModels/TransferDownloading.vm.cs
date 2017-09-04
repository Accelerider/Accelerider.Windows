using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.ViewModels.Others;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.ViewModels
{
    public class TransferDownloadingViewModel : TransferingBaseViewModel
    {
        public TransferDownloadingViewModel(IUnityContainer container) : base(container)
        {
        }

        protected override TransferTaskStatusEnum TransferedStatus => TransferTaskStatusEnum.Checking;

        protected override TransferingTaskList GetTaskList() => Container.Resolve<TransferingTaskList>(TransferingTaskList.DownloadKey);
    }
}

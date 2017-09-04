using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.ViewModels.Others;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.ViewModels
{
    public class TransferUploadingViewModel : TransferingBaseViewModel
    {
        public TransferUploadingViewModel(IUnityContainer container) : base(container)
        {
        }

        protected override TransferTaskStatusEnum TransferedStatus => TransferTaskStatusEnum.Completed;

        protected override TransferingTaskList GetTaskList() => Container.Resolve<TransferingTaskList>(TransferingTaskList.UploadKey);
    }
}

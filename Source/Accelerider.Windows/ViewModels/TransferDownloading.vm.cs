using Accelerider.Windows.ViewModels.Others;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.ViewModels
{
    public class TransferDownloadingViewModel : TransferingBaseViewModel
    {
        public TransferDownloadingViewModel(IUnityContainer container) : base(container)
        {
        }

        protected override TransferingTaskList GetTaskList() => Container.Resolve<TransferingTaskList>(TransferingTaskList.DownloadKey);
    }
}

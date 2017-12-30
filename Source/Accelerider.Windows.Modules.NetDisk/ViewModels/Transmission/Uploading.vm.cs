using Accelerider.Windows.ViewModels.Others;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.ViewModels.Transmission
{
    public class UploadingViewModel : TransferringBaseViewModel
    {
        public UploadingViewModel(IUnityContainer container) : base(container)
        {
        }

        protected override TransferringTaskList GetTaskList() => Container.Resolve<TransferringTaskList>(TransferringTaskList.UploadKey);
    }
}

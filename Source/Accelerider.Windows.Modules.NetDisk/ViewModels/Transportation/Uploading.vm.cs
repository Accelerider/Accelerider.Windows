using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Modules.NetDisk.Extensions;
using Accelerider.Windows.Modules.NetDisk.Models;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.Modules.NetDisk.ViewModels.Transportation
{
    public class UploadingViewModel : TransferringBaseViewModel
    {
        public UploadingViewModel(IUnityContainer container) : base(container)
        {
        }

        protected override ObservableSortedCollection<TransferItem> GetTaskList() => 
            new ObservableSortedCollection<TransferItem>(AcceleriderUser.GetDonwloadItems(), DefaultTransferItemComparer);
    }
}

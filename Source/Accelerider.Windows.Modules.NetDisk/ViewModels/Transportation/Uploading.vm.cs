using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Modules.NetDisk.Extensions;
using Accelerider.Windows.Modules.NetDisk.Models;
using Autofac;


namespace Accelerider.Windows.Modules.NetDisk.ViewModels.Transportation
{
    public class UploadingViewModel : TransferringBaseViewModel
    {
        public UploadingViewModel(IContainer container) : base(container)
        {
        }

        protected override ObservableSortedCollection<TransferItem> GetTaskList() => 
            new ObservableSortedCollection<TransferItem>(AcceleriderUser.GetDonwloadItems(), DefaultTransferItemComparer);
    }
}

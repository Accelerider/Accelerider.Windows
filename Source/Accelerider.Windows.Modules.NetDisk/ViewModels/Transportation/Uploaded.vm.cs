using System.Collections.Generic;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Modules.NetDisk.Interfaces;
using Unity;


namespace Accelerider.Windows.Modules.NetDisk.ViewModels.Transportation
{
    public class UploadedViewModel : TransferredBaseViewModel
    {
        public UploadedViewModel(IUnityContainer container) : base(container)
        {
        }

        protected override ObservableSortedCollection<ILocalDiskFile> GetTransferredFiles() => 
            new ObservableSortedCollection<ILocalDiskFile>(AcceleriderUser.GetUploadedFiles(), DefaultTransferredFileComparer);
    }
}

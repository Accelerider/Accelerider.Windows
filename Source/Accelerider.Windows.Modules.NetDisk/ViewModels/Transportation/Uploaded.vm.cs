using System.Collections.Generic;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Modules.NetDisk.Extensions;
using Accelerider.Windows.Modules.NetDisk.Interfaces;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.Modules.NetDisk.ViewModels.Transportation
{
    public class UploadedViewModel : TransferredBaseViewModel
    {
        public UploadedViewModel(IUnityContainer container) : base(container)
        {
        }

        protected override IEnumerable<ITransferredFile> GetTransferredFiles() => 
            new ObservableSortedCollection<ITransferredFile>(AcceleriderUser.GetUploadedFiles(), DefaultTransferredFileComparer);
    }
}

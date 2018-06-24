using System.Collections.Generic;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Modules.NetDisk.Extensions;
using Accelerider.Windows.Modules.NetDisk.Interfaces;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.Modules.NetDisk.ViewModels.Transportation
{
    public class DownloadedViewModel : TransferredBaseViewModel
    { 
        public DownloadedViewModel(IUnityContainer container) : base(container)
        {
        }

        protected override IEnumerable<ITransferredFile> GetTransferredFiles() => 
            new ObservableSortedCollection<ITransferredFile>(AcceleriderUser.GetDownloadedFiles(), DefaultTransferredFileComparer);
    }
}

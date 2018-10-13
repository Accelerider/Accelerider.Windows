using System.Collections.Generic;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Modules.NetDisk.Extensions;
using Accelerider.Windows.Modules.NetDisk.Interfaces;
using Autofac;


namespace Accelerider.Windows.Modules.NetDisk.ViewModels.Transportation
{
    public class DownloadedViewModel : TransferredBaseViewModel
    { 
        public DownloadedViewModel(IContainer container) : base(container)
        {
        }

        protected override IEnumerable<ITransferredFile> GetTransferredFiles() => 
            new ObservableSortedCollection<ITransferredFile>(AcceleriderUser.GetDownloadedFiles(), DefaultTransferredFileComparer);
    }
}

using System.Collections.ObjectModel;
using Accelerider.Windows.Infrastructure.Interfaces;
using Accelerider.Windows.Modules.NetDisk.ViewModels.Others;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.Modules.NetDisk.ViewModels.Transportation
{
    public class UploadedViewModel : TransportedBaseViewModel
    {
        public UploadedViewModel(IUnityContainer container) : base(container)
        {
        }

        protected override ObservableCollection<ITransferredFile> GetTransferredFiles() => Container.Resolve<TransferringTaskList>(TransferringTaskList.UploadKey).TransferredFileList;
    }
}

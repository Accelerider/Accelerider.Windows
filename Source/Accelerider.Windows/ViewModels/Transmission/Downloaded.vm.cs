using System.Collections.ObjectModel;

using Accelerider.Windows.Infrastructure.Interfaces;
using Accelerider.Windows.ViewModels.Others;

using Microsoft.Practices.Unity;

namespace Accelerider.Windows.ViewModels.Transmission
{
    public class DownloadedViewModel : TransferredBaseViewModel
    {
        public DownloadedViewModel(IUnityContainer container) : base(container)
        {
        }

        protected override ObservableCollection<ITransferredFile> GetTransferredFiles() => Container.Resolve<TransferringTaskList>(TransferringTaskList.DownloadKey).TransferredFileList;
    }
}

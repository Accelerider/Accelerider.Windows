using System.Collections.ObjectModel;
using Accelerider.Windows.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;
using Accelerider.Windows.ViewModels.Others;

namespace Accelerider.Windows.ViewModels.Transmission
{
    public class DownloadedViewModel : TransferredBaseViewModel
    {
        public DownloadedViewModel(IUnityContainer container) : base(container)
        {
        }

        protected override ObservableCollection<ITransferedFile> GetTransferedFiles() => Container.Resolve<TransferingTaskList>(TransferingTaskList.DownloadKey).TransferedFileList;
    }
}

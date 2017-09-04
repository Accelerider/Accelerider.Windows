using System.Collections.ObjectModel;
using Accelerider.Windows.Infrastructure.Interfaces;
using Accelerider.Windows.ViewModels.Others;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.ViewModels
{
    public class TransferUploadedViewModel : TransferedBaseViewModel
    {
        public TransferUploadedViewModel(IUnityContainer container) : base(container)
        {
        }

        protected override ObservableCollection<ITransferedFile> GetTransferedFiles() => Container.Resolve<TransferingTaskList>(TransferingTaskList.UploadKey).TransferedFileList;
    }
}

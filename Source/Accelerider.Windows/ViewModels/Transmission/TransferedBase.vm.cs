using System.Collections.ObjectModel;
using Accelerider.Windows.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.ViewModels.Transmission
{
    public abstract class TransferredBaseViewModel : ViewModelBase
    {
        private ObservableCollection<ITransferredFile> transferredFiles;


        protected TransferredBaseViewModel(IUnityContainer container) : base(container)
        {
            TransferredFiles = GetTransferredFiles();
        }


        public ObservableCollection<ITransferredFile> TransferredFiles
        {
            get => transferredFiles;
            set => SetProperty(ref transferredFiles, value);
        }

        protected abstract ObservableCollection<ITransferredFile> GetTransferredFiles();
    }
}

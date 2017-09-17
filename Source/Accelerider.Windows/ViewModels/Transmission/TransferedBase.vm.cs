using System.Collections.ObjectModel;
using Accelerider.Windows.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.ViewModels.Transmission
{
    public abstract class TransferredBaseViewModel : ViewModelBase
    {
        private ObservableCollection<ITransferedFile> _transferedFiles;


        protected TransferredBaseViewModel(IUnityContainer container) : base(container)
        {
            TransferedFiles = GetTransferedFiles();
        }


        public ObservableCollection<ITransferedFile> TransferedFiles
        {
            get => _transferedFiles;
            set => SetProperty(ref _transferedFiles, value);
        }

        protected abstract ObservableCollection<ITransferedFile> GetTransferedFiles();
    }
}

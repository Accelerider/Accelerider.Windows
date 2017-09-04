using Microsoft.Practices.Unity;
using Accelerider.Windows.Infrastructure.Interfaces;
using System.Collections.ObjectModel;

namespace Accelerider.Windows.ViewModels
{
    public abstract class TransferedBaseViewModel : ViewModelBase
    {
        private ObservableCollection<ITransferedFile> _transferedFiles;


        protected TransferedBaseViewModel(IUnityContainer container) : base(container)
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Core.Files;
using Accelerider.Windows.Events;
using Accelerider.Windows.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.ViewModels
{
    public class TransferUploadedViewModel : ViewModelBase
    {
        private ObservableCollection<ITransferedFile> _transferedFiles;

        public TransferUploadedViewModel(IUnityContainer container) : base(container)
        {
            TransferedFiles = new ObservableCollection<ITransferedFile>(AcceleriderUser.GetUploadedFiles());
            EventAggregator.GetEvent<UploadTaskCompletedEvent>().Subscribe(OnTransferStateChanged);
        }

        private void OnTransferStateChanged(IDiskFile e)
        {
            TransferedFiles.Insert(0, new TransferedFile
            {
                CompletedTime = DateTime.Now,
                FilePath = e.FilePath,
                FileSize = e.FileSize
            });
        }

        public ObservableCollection<ITransferedFile> TransferedFiles
        {
            get => _transferedFiles;
            set => SetProperty(ref _transferedFiles, value);
        }
    }
}

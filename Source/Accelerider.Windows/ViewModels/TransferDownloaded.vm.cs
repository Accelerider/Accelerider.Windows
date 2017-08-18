using System;
using System.Collections.ObjectModel;
using Accelerider.Windows.Core.Files;
using Accelerider.Windows.Events;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.ViewModels
{
    public class TransferDownloadedViewModel : ViewModelBase
    {
        private ObservableCollection<ITransferedFile> _transferedFiles;


        public TransferDownloadedViewModel(IUnityContainer container) : base(container)
        {
            TransferedFiles = new ObservableCollection<ITransferedFile>(AcceleriderUser.GetDownloadedFiles());
            EventAggregator.GetEvent<DownloadTaskTranferedEvent>().Subscribe(OnTransferStateChanged);
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

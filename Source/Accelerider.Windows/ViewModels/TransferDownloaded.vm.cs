using System;
using System.Collections.ObjectModel;
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
            EventAggregator.GetEvent<TransferStateChangedEvent>().Subscribe(OnTransferStateChanged, e => e.NewState == TransferStateEnum.Checking);
        }

        private void OnTransferStateChanged(TransferStateChangedEventArgs e)
        {

        }

        public ObservableCollection<ITransferedFile> TransferedFiles
        {
            get => _transferedFiles;
            set => SetProperty(ref _transferedFiles, value);
        }
    }
}

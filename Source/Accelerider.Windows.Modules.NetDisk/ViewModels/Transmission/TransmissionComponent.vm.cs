using System.Linq;
using System.Windows.Input;
using Accelerider.Windows.Infrastructure.Commands;
using Accelerider.Windows.Modules.NetDisk.ViewModels.Others;
using Accelerider.Windows.Modules.NetDisk.Views.Dialogs;
using MaterialDesignThemes.Wpf;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.Modules.NetDisk.ViewModels.Transmission
{
    public class TransmissionComponentViewModel : ViewModelBase
    {
        private ICommand _addOfflineTaskCommand;
        private TransferringTaskList _downloadList;
        private TransferringTaskList _uploadList;

        public TransmissionComponentViewModel(IUnityContainer container) : base(container)
        {
            AddOfflineTaskCommand = new RelayCommand(AddOfflineTaskCommandExecute);

            ConfigureTransferList();
        }

        public ICommand AddOfflineTaskCommand
        {
            get => _addOfflineTaskCommand;
            set => SetProperty(ref _addOfflineTaskCommand, value);
        }

        public TransferringTaskList DownloadList
        {
            get => _downloadList;
            set => SetProperty(ref _downloadList, value);
        }

        public TransferringTaskList UploadList
        {
            get => _uploadList;
            set => SetProperty(ref _uploadList, value);
        }

        private async void AddOfflineTaskCommandExecute() => await DialogHost.Show(new AddOfflineTaskDialog(), "RootDialog");

        private void ConfigureTransferList()
        {
            DownloadList = new TransferringTaskList(AcceleriderUser.GetDownloadingTasks().Select(task => new TransferringTaskViewModel(task)));
            UploadList = new TransferringTaskList(AcceleriderUser.GetUploadingTasks().Select(task => new TransferringTaskViewModel(task)));

            DownloadList.TransferredFileList = new TransferredFileList(AcceleriderUser.GetDownloadedFiles());
            UploadList.TransferredFileList = new TransferredFileList(AcceleriderUser.GetUploadedFiles());

            Container.RegisterInstance(TransferringTaskList.DownloadKey, DownloadList);
            Container.RegisterInstance(TransferringTaskList.UploadKey, UploadList);
        }
    }
}

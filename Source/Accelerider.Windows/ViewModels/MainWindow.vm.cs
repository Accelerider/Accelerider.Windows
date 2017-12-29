using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Accelerider.Windows.Assets;
using Accelerider.Windows.Commands;
using Accelerider.Windows.Common;
using Accelerider.Windows.ViewModels.Others;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private TransferringTaskList _downloadList;
        private TransferringTaskList _uploadList;
        private ICommand _feedbackCommand;


        public MainWindowViewModel(IUnityContainer container) : base(container)
        {
            FeedbackCommand = new RelayCommand(() => Process.Start(ConstStrings.IssueUrl));

            GlobalMessageQueue.Enqueue(UiStrings.Message_Welcome);

            ConfigureTransferList();
        }

        public ICommand FeedbackCommand
        {
            get => _feedbackCommand;
            set => SetProperty(ref _feedbackCommand, value);
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


        public override void OnUnloaded(object view)
        {
            AcceleriderUser.OnExit();
        }


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

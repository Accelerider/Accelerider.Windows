using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Accelerider.Windows.Events;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;
using Accelerider.Windows.ViewModels.Items;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.ViewModels
{
    public class TransferUploadingViewModel : ViewModelBase
    {
        private ObservableCollection<TransferTaskViewModel> _uploadTasks;


        public TransferUploadingViewModel(IUnityContainer container) : base(container)
        {
            UploadTasks = new ObservableCollection<TransferTaskViewModel>(NetDiskUser.GetUploadingFiles().Select(item =>
            {
                item.TransferStateChanged += OnUploaded;
                return new TransferTaskViewModel(new TaskCreatedEventArgs(NetDiskUser.Username, item));
            }));

            EventAggregator.GetEvent<UploadTaskCreatedEvent>().Subscribe(OnUploadTaskCreated, token => token != null && token.Any());
        }


        public ObservableCollection<TransferTaskViewModel> UploadTasks
        {
            get => _uploadTasks;
            set => SetProperty(ref _uploadTasks, value);
        }


        private void OnUploadTaskCreated(IReadOnlyCollection<TaskCreatedEventArgs> taskInfos)
        {
            foreach (var taskInfo in taskInfos)
            {
                taskInfo.Token.TransferStateChanged += OnUploaded;
                UploadTasks.Add(new TransferTaskViewModel(taskInfo));
            }
        }

        private void OnUploaded(object sender, TransferStateChangedEventArgs e)
        {
            if (e.NewState != TransferStateEnum.Checking) return;

            var temp = UploadTasks.FirstOrDefault(item => item.FileInfo.FilePath.FullPath == e.Token.FileInfo.FilePath.FullPath);
            if (temp != null)
            {
                UploadTasks.Remove(temp);
            }
        }
    }
}

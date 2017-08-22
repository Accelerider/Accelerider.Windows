using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;
using System.Collections.ObjectModel;
using Accelerider.Windows.ViewModels.Items;
using Accelerider.Windows.Events;
using Accelerider.Windows.Infrastructure.Interfaces;
using Accelerider.Windows.Infrastructure;

namespace Accelerider.Windows.ViewModels
{
    public abstract class TransferingBaseViewModel<T> : ViewModelBase
        where T : TaskCreatedEvent, new()
    {
        private ObservableCollection<TransferTaskViewModel> _transferTasks;


        public TransferingBaseViewModel(IUnityContainer container) : base(container)
        {
            TransferTasks = new ObservableCollection<TransferTaskViewModel>(GetinitializedTasks().Select(item =>
            {
                item.TransferStateChanged += OnTransfered;
                return new TransferTaskViewModel(new TaskCreatedEventArgs(NetDiskUser.Username, item));
            }));

            EventAggregator.GetEvent<T>().Subscribe(OnTransferTaskCreated, token => token != null && token.Any());
        }


        public ObservableCollection<TransferTaskViewModel> TransferTasks
        {
            get { return _transferTasks; }
            set { SetProperty(ref _transferTasks, value); }
        }


        private void OnTransferTaskCreated(IReadOnlyCollection<TaskCreatedEventArgs> taskInfos)
        {
            foreach (var taskInfo in taskInfos)
            {
                taskInfo.Token.TransferStateChanged += OnTransfered;
                TransferTasks.Add(new TransferTaskViewModel(taskInfo));
            }
        }

        private void OnTransfered(object sender, TransferStateChangedEventArgs e)
        {
            if (e.NewState != TransferStateEnum.Checking) return;

            var temp = TransferTasks.FirstOrDefault(item => item.FileInfo.FilePath.FullPath == e.Token.FileInfo.FilePath.FullPath);
            if (temp != null)
            {
                TransferTasks.Remove(temp);
            }
        }

        protected abstract IReadOnlyCollection<ITransferTaskToken> GetinitializedTasks();

    }
}

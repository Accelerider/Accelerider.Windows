using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Accelerider.Windows.Commands;
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
        private ICommand _pauseCommand;
        private ICommand _startCommand;
        private ICommand _startForceCommand;
        private ICommand _cancelCommand;


        protected TransferingBaseViewModel(IUnityContainer container) : base(container)
        {
            InitializeCommands();

            TransferTasks = new ObservableCollection<TransferTaskViewModel>(GetinitializedTasks().Select(item =>
            {
                item.TransferStateChanged += OnTransfered;
                return new TransferTaskViewModel(new TaskCreatedEventArgs(NetDiskUser.Username, item));
            }));

            EventAggregator.GetEvent<T>().Subscribe(OnTransferTaskCreated, token => token != null && token.Any());
        }


        public ObservableCollection<TransferTaskViewModel> TransferTasks
        {
            get => _transferTasks;
            set => SetProperty(ref _transferTasks, value);
        }

        #region Commands
        public ICommand PauseCommand
        {
            get => _pauseCommand;
            set => SetProperty(ref _pauseCommand, value);
        }

        public ICommand StartCommand
        {
            get => _startCommand;
            set => SetProperty(ref _startCommand, value);
        }

        public ICommand StartForceCommand
        {
            get => _startForceCommand;
            set => SetProperty(ref _startForceCommand, value);
        }

        public ICommand CancelCommand
        {
            get => _cancelCommand;
            set => SetProperty(ref _cancelCommand, value);
        }


        private void InitializeCommands()
        {
            PauseCommand = new RelayCommand<TransferTaskViewModel>(
                taskToken => OperateTaskToken(taskToken, token => token.PauseAsync(), "Pause task failed."),
                taskToken => !taskToken.IsBusy && taskToken.Token.TransferState == TransferStateEnum.Transfering);
            StartCommand = new RelayCommand<TransferTaskViewModel>(
                taskToken => OperateTaskToken(taskToken, token => token.StartAsync(), "Restart task failed."),
                taskToken => !taskToken.IsBusy && taskToken.Token.TransferState == TransferStateEnum.Paused);
            StartForceCommand = new RelayCommand<TransferTaskViewModel>(
                taskToken => OperateTaskToken(taskToken, token => token.StartAsync(true), "Jump queue failed."),
                taskToken => !taskToken.IsBusy && taskToken.Token.TransferState != TransferStateEnum.Transfering);
            CancelCommand = new RelayCommand<TransferTaskViewModel>(
                taskToken => OperateTaskToken(taskToken, token => token.CancelAsync(), "Cancel task failed."),
                taskToken => !taskToken.IsBusy);
        }

        private async void OperateTaskToken(TransferTaskViewModel taskToken, Func<ITransferTaskToken, Task<bool>> operation, string errorMessage)
        {
            taskToken.IsBusy = true;
            if (!await operation(taskToken.Token)) GlobalMessageQueue.Enqueue(errorMessage);
            taskToken.IsBusy = false;
        }
        #endregion

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

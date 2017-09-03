using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Accelerider.Windows.Commands;
using Accelerider.Windows.Events;
using Accelerider.Windows.Infrastructure.Interfaces;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.ViewModels.Others;

namespace Accelerider.Windows.ViewModels
{
    public abstract class TransferingBaseViewModel<T> : ViewModelBase
        where T : TaskCreatedEvent, new()
    {
        private AutoOrderedTaskList _transferTasks;
        private ICommand _pauseCommand;
        private ICommand _startCommand;
        private ICommand _startForceCommand;
        private ICommand _cancelCommand;


        protected TransferingBaseViewModel(IUnityContainer container) : base(container)
        {
            InitializeCommands();

            TransferTasks = new AutoOrderedTaskList(GetInitializedTasks().Select(item =>
            {
                item.TransferTaskStatusChanged += OnTransfered;
                return new TransferTaskViewModel(item);
            }));

            EventAggregator.GetEvent<T>().Subscribe(OnTransferTaskCreated, token => token != null);
        }


        protected abstract TransferTaskStatusEnum TransferedStatus { get; }

        public AutoOrderedTaskList TransferTasks
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
                taskToken => !taskToken.IsBusy && taskToken.Token.TaskStatus == TransferTaskStatusEnum.Transfering);
            StartCommand = new RelayCommand<TransferTaskViewModel>(
                taskToken => OperateTaskToken(taskToken, token => token.StartAsync(), "Restart task failed."),
                taskToken => !taskToken.IsBusy && taskToken.Token.TaskStatus == TransferTaskStatusEnum.Paused);
            StartForceCommand = new RelayCommand<TransferTaskViewModel>(
                taskToken => OperateTaskToken(taskToken, token => token.StartAsync(true), "Jump queue failed."),
                taskToken => !taskToken.IsBusy && taskToken.Token.TaskStatus != TransferTaskStatusEnum.Transfering);
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

        private void OnTransferTaskCreated(ITransferTaskToken token)
        {
            token.TransferTaskStatusChanged += OnTransfered;
            TransferTasks.Add(new TransferTaskViewModel(token));
        }

        private void OnTransfered(object sender, TransferTaskStatusChangedEventArgs e)
        {
            if (e.NewStatus != TransferedStatus) return;

            var temp = TransferTasks.FirstOrDefault(item => item.FileSummary.FilePath.FullPath == e.Token.FileSummary.FilePath.FullPath);
            if (temp != null)
            {
                Application.Current.Dispatcher.Invoke(() => TransferTasks.Remove(temp));
            }
        }

        protected abstract IReadOnlyCollection<ITransferTaskToken> GetInitializedTasks();

    }
}

using System;
using Microsoft.Practices.Unity;
using System.Threading.Tasks;
using System.Windows.Input;
using Accelerider.Windows.Commands;
using Accelerider.Windows.Infrastructure.Interfaces;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.ViewModels.Others;

namespace Accelerider.Windows.ViewModels
{
    public abstract class TransferingBaseViewModel : ViewModelBase
    {
        private TransferingTaskList _transferTasks;
        private ICommand _pauseCommand;
        private ICommand _startCommand;
        private ICommand _startForceCommand;
        private ICommand _cancelCommand;


        protected TransferingBaseViewModel(IUnityContainer container) : base(container)
        {
            InitializeCommands();

            TransferTasks = GetTaskList();
        }


        public TransferingTaskList TransferTasks
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
            PauseCommand = new RelayCommand<TransferingTaskViewModel>(
                taskToken => OperateTaskToken(taskToken, token => token.PauseAsync(), "Pause task failed."),
                taskToken => !taskToken.IsBusy && 
                taskToken.Token.TaskStatus == TransferTaskStatusEnum.Transferring ||
                taskToken.Token.TaskStatus == TransferTaskStatusEnum.Waiting);
            StartCommand = new RelayCommand<TransferingTaskViewModel>(
                taskToken => OperateTaskToken(taskToken, token => token.StartAsync(), "Restart task failed."),
                taskToken => !taskToken.IsBusy && taskToken.Token.TaskStatus == TransferTaskStatusEnum.Paused);
            StartForceCommand = new RelayCommand<TransferingTaskViewModel>(
                taskToken => OperateTaskToken(taskToken, token => token.StartAsync(true), "Jump queue failed."),
                taskToken => !taskToken.IsBusy && taskToken.Token.TaskStatus != TransferTaskStatusEnum.Transferring);
            CancelCommand = new RelayCommand<TransferingTaskViewModel>(
                taskToken => OperateTaskToken(taskToken, token => token.CancelAsync(), "Cancel task failed."),
                taskToken => !taskToken.IsBusy);
        }

        private async void OperateTaskToken(TransferingTaskViewModel taskToken, Func<ITransferTaskToken, Task<bool>> operation, string errorMessage)
        {
            taskToken.IsBusy = true;
            if (!await operation(taskToken.Token)) GlobalMessageQueue.Enqueue(errorMessage);
            taskToken.IsBusy = false;
        }
        #endregion

        protected abstract TransferingTaskList GetTaskList();
    }
}

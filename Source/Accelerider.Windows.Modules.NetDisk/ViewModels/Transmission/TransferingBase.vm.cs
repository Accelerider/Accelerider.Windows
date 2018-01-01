using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Commands;
using Accelerider.Windows.Infrastructure.Interfaces;
using Accelerider.Windows.Modules.NetDisk.ViewModels.Others;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.Modules.NetDisk.ViewModels.Transmission
{
    public abstract class TransferringBaseViewModel : ViewModelBase
    {
        private TransferringTaskList _transferTasks;
        private RelayCommand<TransferringTaskViewModel> _pauseCommand;
        private RelayCommand<TransferringTaskViewModel> _startCommand;
        private RelayCommand<TransferringTaskViewModel> _startForceCommand;
        private RelayCommand<TransferringTaskViewModel> _cancelCommand;


        protected TransferringBaseViewModel(IUnityContainer container) : base(container)
        {
            InitializeCommands();

            TransferTasks = GetTaskList();
        }


        public TransferringTaskList TransferTasks
        {
            get => _transferTasks;
            set => SetProperty(ref _transferTasks, value);
        }

        #region Commands
        public RelayCommand<TransferringTaskViewModel> PauseCommand
        {
            get => _pauseCommand;
            set => SetProperty(ref _pauseCommand, value);
        }

        public RelayCommand<TransferringTaskViewModel> StartCommand
        {
            get => _startCommand;
            set => SetProperty(ref _startCommand, value);
        }

        public RelayCommand<TransferringTaskViewModel> StartForceCommand
        {
            get => _startForceCommand;
            set => SetProperty(ref _startForceCommand, value);
        }

        public RelayCommand<TransferringTaskViewModel> CancelCommand
        {
            get => _cancelCommand;
            set => SetProperty(ref _cancelCommand, value);
        }


        private void InitializeCommands()
        {
            PauseCommand = new RelayCommand<TransferringTaskViewModel>(
                taskToken => OperateTaskToken(taskToken, token => token.PauseAsync(), "Pause task failed."),
                taskToken => !taskToken.IsBusy &&
                taskToken.Token.TaskStatus == TransferTaskStatusEnum.Transferring ||
                taskToken.Token.TaskStatus == TransferTaskStatusEnum.Waiting);
            StartCommand = new RelayCommand<TransferringTaskViewModel>(
                taskToken => OperateTaskToken(taskToken, token => token.StartAsync(), "Restart task failed."),
                taskToken => !taskToken.IsBusy && taskToken.Token.TaskStatus == TransferTaskStatusEnum.Paused);
            StartForceCommand = new RelayCommand<TransferringTaskViewModel>(
                taskToken => OperateTaskToken(taskToken, token => token.StartAsync(true), "Jump queue failed."),
                taskToken => !taskToken.IsBusy && taskToken.Token.TaskStatus != TransferTaskStatusEnum.Transferring);
            CancelCommand = new RelayCommand<TransferringTaskViewModel>(
                taskToken => OperateTaskToken(taskToken, token => token.CancelAsync(), "Cancel task failed."),
                taskToken => !taskToken.IsBusy);
        }

        private async void OperateTaskToken(TransferringTaskViewModel taskToken, Func<ITransferTaskToken, Task<bool>> operation, string errorMessage)
        {
            taskToken.IsBusy = true;
            if (!await operation(taskToken.Token)) GlobalMessageQueue.Enqueue(errorMessage);
            taskToken.IsBusy = false;
        }
        #endregion

        protected abstract TransferringTaskList GetTaskList();
    }
}

using System;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Commands;
using Accelerider.Windows.Infrastructure.FileTransferService;
using Accelerider.Windows.Infrastructure.Interfaces;
using Accelerider.Windows.Modules.NetDisk.ViewModels.Others;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.Modules.NetDisk.ViewModels.Transportation
{
    public abstract class TransportingBaseViewModel : ViewModelBase
    {
        private TransferringTaskList _transferTasks;
        private RelayCommand<TransportingTaskItem> _pauseCommand;
        private RelayCommand<TransportingTaskItem> _startCommand;
        private RelayCommand<TransportingTaskItem> _startForceCommand;
        private RelayCommand<TransportingTaskItem> _cancelCommand;


        protected TransportingBaseViewModel(IUnityContainer container) : base(container)
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
        public RelayCommand<TransportingTaskItem> PauseCommand
        {
            get => _pauseCommand;
            set => SetProperty(ref _pauseCommand, value);
        }

        public RelayCommand<TransportingTaskItem> StartCommand
        {
            get => _startCommand;
            set => SetProperty(ref _startCommand, value);
        }

        public RelayCommand<TransportingTaskItem> StartForceCommand
        {
            get => _startForceCommand;
            set => SetProperty(ref _startForceCommand, value);
        }

        public RelayCommand<TransportingTaskItem> CancelCommand
        {
            get => _cancelCommand;
            set => SetProperty(ref _cancelCommand, value);
        }


        private void InitializeCommands()
        {
            PauseCommand = new RelayCommand<TransportingTaskItem>(
                taskToken => OperateTaskToken(taskToken, token => token.PauseAsync(), "Pause task failed."),
                taskToken => !taskToken.IsBusy &&
                taskToken.Token.TaskStatus == TransferStatus.Transferring ||
                taskToken.Token.TaskStatus == TransferStatus.Waiting);
            StartCommand = new RelayCommand<TransportingTaskItem>(
                taskToken => OperateTaskToken(taskToken, token => token.Start(), "Restart task failed."),
                taskToken => !taskToken.IsBusy && taskToken.Token.TaskStatus == TransferTaskStatusEnum.Paused);
            StartForceCommand = new RelayCommand<TransportingTaskItem>(
                taskToken => OperateTaskToken(taskToken, token => token.Start(true), "Jump queue failed."),
                taskToken => !taskToken.IsBusy && taskToken.Token.TaskStatus != TransferTaskStatusEnum.Transferring);
            CancelCommand = new RelayCommand<TransportingTaskItem>(
                taskToken => OperateTaskToken(taskToken, token => token.DisposeAsync(), "Cancel task failed."),
                taskToken => !taskToken.IsBusy);
        }

        private async void OperateTaskToken(TransportingTaskItem taskToken, Func<ITransporter, Task<bool>> operation, string errorMessage)
        {
            taskToken.IsBusy = true;
            if (!await operation(taskToken.Token)) GlobalMessageQueue.Enqueue(errorMessage);
            taskToken.IsBusy = false;
        }
        #endregion

        protected abstract TransferringTaskList GetTaskList();
    }
}

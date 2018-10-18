using System;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Mvvm;
using Accelerider.Windows.Infrastructure.TransferService;
using Accelerider.Windows.Modules.NetDisk.Models;
using Autofac;


namespace Accelerider.Windows.Modules.NetDisk.ViewModels.Transportation
{
    public abstract class TransferringBaseViewModel : ViewModelBase, IAwareViewLoadedAndUnloaded
    {
        private ObservableSortedCollection<TransferItem> _transferTasks;
        private RelayCommand<TransferItem> _pauseCommand;
        private RelayCommand<TransferItem> _startCommand;
        private RelayCommand<TransferItem> _startForceCommand;
        private RelayCommand<TransferItem> _cancelCommand;


        protected TransferringBaseViewModel(IContainer container) : base(container)
        {
            InitializeCommands();
        }

        public void OnLoaded()
        {
            if (TransferTasks == null)
                TransferTasks = GetTaskList();
        }

        public void OnUnloaded()
        {
        }

        protected Comparison<TransferItem> DefaultTransferItemComparer { get; } // TODO

        public ObservableSortedCollection<TransferItem> TransferTasks
        {
            get => _transferTasks;
            set => SetProperty(ref _transferTasks, value);
        }

        #region Commands

        public RelayCommand<TransferItem> PauseCommand
        {
            get => _pauseCommand;
            set => SetProperty(ref _pauseCommand, value);
        }

        public RelayCommand<TransferItem> StartCommand
        {
            get => _startCommand;
            set => SetProperty(ref _startCommand, value);
        }

        public RelayCommand<TransferItem> StartForceCommand
        {
            get => _startForceCommand;
            set => SetProperty(ref _startForceCommand, value);
        }

        public RelayCommand<TransferItem> CancelCommand
        {
            get => _cancelCommand;
            set => SetProperty(ref _cancelCommand, value);
        }


        private void InitializeCommands()
        {
            PauseCommand = new RelayCommand<TransferItem>(
                item => OperateTaskToken(item, token => token.Suspend(), "Pause task failed."), // TODO: [I18N]
                item => item.Transporter.Status == TransferStatus.Transferring ||
                        item.Transporter.Status == TransferStatus.Ready);

            StartCommand = new RelayCommand<TransferItem>(
                item => OperateTaskToken(item, token => token.Ready(), "Restart task failed."),
                item => item.Transporter.Status == TransferStatus.Suspended);

            StartForceCommand = new RelayCommand<TransferItem>(
                item => OperateTaskToken(item, token => token.AsNext(), "Jump queue failed."),
                item => item.Transporter.Status != TransferStatus.Transferring);

            CancelCommand = new RelayCommand<TransferItem>(
                item => OperateTaskToken(item, token => token.Dispose(), "Cancel task failed."));
        }

        private void OperateTaskToken(TransferItem item, Action<IManagedTransporterToken> operation, string errorMessage)
        {
            try
            {
                operation?.Invoke(item.ManagedToken);
            }
            catch
            {
                GlobalMessageQueue.Enqueue(errorMessage);
            }
        }

        #endregion

        protected abstract ObservableSortedCollection<TransferItem> GetTaskList();
    }
}

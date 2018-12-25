using System;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Mvvm;
using Accelerider.Windows.Modules.NetDisk.Models;
using Accelerider.Windows.TransferService;
using MaterialDesignThemes.Wpf;
using Unity;


namespace Accelerider.Windows.Modules.NetDisk.ViewModels.Transportation
{
    public abstract class TransferringBaseViewModel : ViewModelBase, IViewLoadedAndUnloadedAware, INotificable
    {
        private ObservableSortedCollection<TransferItem> _transferTasks;

        public ObservableSortedCollection<TransferItem> TransferTasks
        {
            get => _transferTasks;
            set => SetProperty(ref _transferTasks, value);
        }

        public ISnackbarMessageQueue GlobalMessageQueue { get; set; }

        protected TransferringBaseViewModel(IUnityContainer container) : base(container)
        {
            InitializeCommands();
        }

        public virtual void OnLoaded()
        {
            if (TransferTasks == null) TransferTasks = GetTaskList();
        }

        public virtual void OnUnloaded() { }

        #region Commands

        public RelayCommand<TransferItem> PauseCommand { get; private set; }

        public RelayCommand<TransferItem> StartCommand { get; private set; }

        public RelayCommand<TransferItem> StartForceCommand { get; private set; }

        public RelayCommand<TransferItem> CancelCommand { get; private set; }

        private void InitializeCommands()
        {
            PauseCommand = new RelayCommand<TransferItem>(
                item => OperateTaskToken(item, token => token.Suspend(), "Pause task failed."), // TODO: [I18N]
                item => item.BindableTransporter.Status == TransferStatus.Transferring ||
                        item.BindableTransporter.Status == TransferStatus.Ready);

            StartCommand = new RelayCommand<TransferItem>(
                item => OperateTaskToken(item, token => token.Ready(), "Restart task failed."),
                item => item.BindableTransporter.Status == TransferStatus.Suspended);

            StartForceCommand = new RelayCommand<TransferItem>(
                item => OperateTaskToken(item, token => token.AsNext(), "Jump queue failed."),
                item => item.BindableTransporter.Status != TransferStatus.Transferring);

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

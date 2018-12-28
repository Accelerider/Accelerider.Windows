using System;
using System.Windows.Input;
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
        private ObservableSortedCollection<IDownloadingFile> _transferTasks;

        public ObservableSortedCollection<IDownloadingFile> TransferTasks
        {
            get => _transferTasks;
            set => SetProperty(ref _transferTasks, value);
        }

        public ISnackbarMessageQueue GlobalMessageQueue { get; set; }

        protected TransferringBaseViewModel(IUnityContainer container) : base(container)
        {
        }

        public virtual void OnLoaded() { }

        public virtual void OnUnloaded() { }

        #region Commands

        public ICommand PauseCommand { get; protected set; }

        public ICommand StartCommand { get; protected set; }

        public ICommand StartForceCommand { get; protected set; }

        public ICommand CancelCommand { get; protected set; }

        #endregion
    }
}

using System.Linq;
using System.Windows.Input;
using Accelerider.Windows.Infrastructure.Commands;
using Accelerider.Windows.Modules.NetDisk.ViewModels.Others;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.Modules.NetDisk.ViewModels.Transmission
{
    public class DownloadingViewModel : TransferringBaseViewModel
    {
        private ICommand _pauseAllCommand;
        private ICommand _cancelAllCommand;

        public DownloadingViewModel(IUnityContainer container) : base(container)
        {
            PauseAllCommand = new RelayCommand(() =>
            {
                foreach (var task in TransferTasks)
                    if (PauseCommand.CanExecute(task))
                        PauseCommand.Execute(task);
            },
            () => TransferTasks?.Any() ?? false);

            CancelAllCommand = new RelayCommand(() =>
            {
                foreach (var task in TransferTasks)
                    if (CancelCommand.CanExecute(task))
                        CancelCommand.Execute(task);
            },
            () => TransferTasks?.Any() ?? false);
        }

        public ICommand PauseAllCommand
        {
            get => _pauseAllCommand;
            set => SetProperty(ref _pauseAllCommand, value);
        }

        public ICommand CancelAllCommand
        {
            get => _cancelAllCommand;
            set => SetProperty(ref _cancelAllCommand, value);
        }

        protected override TransferringTaskList GetTaskList() => Container.Resolve<TransferringTaskList>(TransferringTaskList.DownloadKey);
    }
}

using System.Linq;
using System.Windows.Input;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Commands;
using Accelerider.Windows.Modules.NetDisk.Extensions;
using Accelerider.Windows.Modules.NetDisk.Models;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.Modules.NetDisk.ViewModels.Transportation
{
    public class DownloadingViewModel : TransferringBaseViewModel
    {
        private ICommand _pauseAllCommand;
        private ICommand _cancelAllCommand;

        public DownloadingViewModel(IUnityContainer container) : base(container)
        {
            PauseAllCommand = new RelayCommand(() => TransferTasks.ForEach(item =>
            {
                if (PauseCommand.CanExecute(item)) PauseCommand.Execute(item);
            }),
            () => TransferTasks?.Any() ?? false);

            CancelAllCommand = new RelayCommand(() => TransferTasks.ForEach(item =>
            {
                if (CancelCommand.CanExecute(item)) CancelCommand.Execute(item);
            }),
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

        protected override ObservableSortedCollection<TransferItem> GetTaskList() =>
            new ObservableSortedCollection<TransferItem>(AcceleriderUser.GetDonwloadItems(), DefaultTransferItemComparer);
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Modules.NetDisk.Models;
using Unity;

namespace Accelerider.Windows.Modules.NetDisk.ViewModels.Transportation
{
    public class DownloadingViewModel : TransferringBaseViewModel
    {
        public ICommand PauseAllCommand { get; }

        public ICommand CancelAllCommand { get; }

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

        protected override ObservableSortedCollection<TransferItem> GetTaskList() =>
            new ObservableSortedCollection<TransferItem>(AcceleriderUser.GetDownloadItems(), (x, y) => x.Transporter.Status - y.Transporter.Status);
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Modules.NetDisk.Models;
using Accelerider.Windows.TransferService;
using Accelerider.Windows.TransferService.WpfInteractions;
using Unity;

namespace Accelerider.Windows.Modules.NetDisk.ViewModels.Transportation
{
    public class DownloadingViewModel : TransferringBaseViewModel
    {
        public ICommand PauseAllCommand { get; }

        public ICommand CancelAllCommand { get; }

        public DownloadingViewModel(IUnityContainer container) : base(container)
        {
            PauseAllCommand = new RelayCommand(
                () => TransferTasks.ForEach(item => PauseCommand.Invoke(item)),
                () => TransferTasks?.Any() ?? false);

            CancelAllCommand = new RelayCommand(
                () => TransferTasks.ForEach(item => CancelCommand.Invoke(item)),
                () => TransferTasks?.Any() ?? false);

            EventAggregator.GetEvent<TransferItemsAddedEvent>().Subscribe(
                InitializeTransferItem,
                Prism.Events.ThreadOption.UIThread,
                keepSubscriberReferenceAlive: true,
                filter: _ => TransferTasks != null);
        }

        public override void OnLoaded()
        {
            if (TransferTasks == null)
            {
                TransferTasks = new ObservableSortedCollection<TransferItem>((x, y) => x.BindableTransporter.Status - y.BindableTransporter.Status);

                AcceleriderUser
                    .GetDownloadItems()
                    .ForEach(item =>
                    {
                        if (item.Status == TransferStatus.Completed)
                        {
                            EventAggregator.GetEvent<TransferItemCompletedEvent>().Publish(item.Transporter.Context.LocalPath.ToTransferredFile());
                        }
                        else
                        {
                            InitializeTransferItem(item);
                        }
                    });
            }
        }

        private void InitializeTransferItem(TransferItem item)
        {
            item.Transporter
                .Where(notification => notification.Status == TransferStatus.Disposed)
                .ObserveOn(Dispatcher)
                .Subscribe(notification => TransferTasks.Remove(item), () =>
                {
                    TransferTasks.Remove(item);
                    EventAggregator.GetEvent<TransferItemCompletedEvent>().Publish(item.Transporter.Context.LocalPath.ToTransferredFile());
                });

            TransferTasks.Add(item);
        }

        protected override ObservableSortedCollection<TransferItem> GetTaskList()
        {
            return new ObservableSortedCollection<TransferItem>(
                AcceleriderUser.GetDownloadItems(),
                (x, y) => x.BindableTransporter.Status - y.BindableTransporter.Status);
        }
    }
}

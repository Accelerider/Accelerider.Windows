using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Modules.NetDisk.Models;
using Accelerider.Windows.TransferService;
using Unity;

namespace Accelerider.Windows.Modules.NetDisk.ViewModels.Transportation
{
    public class DownloadingViewModel : TransferringBaseViewModel
    {
        public ICommand PauseAllCommand { get; private set; }

        public ICommand CancelAllCommand { get; private set; }

        public DownloadingViewModel(IUnityContainer container) : base(container)
        {
            InitializeCommands();

            EventAggregator.GetEvent<TransferItemsAddedEvent>().Subscribe(
                InitializeTransferItem,
                Prism.Events.ThreadOption.UIThread,
                keepSubscriberReferenceAlive: true,
                filter: _ => TransferTasks != null);
        }

        private void InitializeCommands()
        {
            PauseCommand = new RelayCommand<IDownloadingFile>(
                item => OperateTaskToken(item, token => token.Suspend(), "Pause task failed."), // TODO: [I18N]
                item => item.DownloadInfo.Status == TransferStatus.Transferring ||
                        item.DownloadInfo.Status == TransferStatus.Ready);

            StartCommand = new RelayCommand<IDownloadingFile>(
                item => OperateTaskToken(item, token => token.Ready(), "Restart task failed."),
                item => item.DownloadInfo.Status == TransferStatus.Suspended);

            StartForceCommand = new RelayCommand<IDownloadingFile>(
                item => OperateTaskToken(item, token => token.AsNext(), "Jump queue failed."),
                item => item.DownloadInfo.Status != TransferStatus.Transferring);

            CancelCommand = new RelayCommand<IDownloadingFile>(
                item => OperateTaskToken(item, token => token.Dispose(), "Cancel task failed."));

            PauseAllCommand = new RelayCommand(
                () => TransferTasks.ForEach(item => PauseCommand.Invoke(item)),
                () => TransferTasks?.Any() ?? false);

            CancelAllCommand = new RelayCommand(
                () => TransferTasks.ForEach(item => CancelCommand.Invoke(item)),
                () => TransferTasks?.Any() ?? false);
        }

        private void OperateTaskToken(IDownloadingFile item, Action<IManagedTransporterToken> operation, string errorMessage)
        {
            try
            {
                operation?.Invoke(item.Operations);
            }
            catch
            {
                GlobalMessageQueue.Enqueue(errorMessage);
            }
        }

        public override void OnLoaded()
        {
            if (TransferTasks == null)
            {
                TransferTasks = new ObservableSortedCollection<IDownloadingFile>((x, y) => x.DownloadInfo.Status - y.DownloadInfo.Status);

                AcceleriderUser
                    .GetNetDiskUsers()
                    .SelectMany(item => item.GetDownloadingFiles())
                    .ForEach(item =>
                    {
                        if (item.DownloadInfo.Status == TransferStatus.Completed)
                        {
                            EventAggregator.GetEvent<TransferItemCompletedEvent>().Publish(item.DownloadInfo.Context.LocalPath.ToTransferredFile());
                        }
                        else
                        {
                            InitializeTransferItem(item);
                        }
                    });
            }
        }

        private void InitializeTransferItem(IDownloadingFile item)
        {
            item.DownloadInfo
                .Where(notification => notification.Status == TransferStatus.Disposed)
                .ObserveOn(Dispatcher)
                .Subscribe(
                    _ => TransferTasks.Remove(item),
                    () =>
                    {
                        TransferTasks.Remove(item);
                        EventAggregator
                            .GetEvent<TransferItemCompletedEvent>()
                            .Publish(item.DownloadInfo.Context.LocalPath.ToTransferredFile());
                    });

            TransferTasks.Add(item);
        }
    }
}

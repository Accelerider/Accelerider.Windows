using System.Net;
using Accelerider.Windows.ViewModels;
using Microsoft.Practices.Unity;
using Accelerider.Windows.Assets;
using Accelerider.Windows.Events;
using Accelerider.Windows.Infrastructure.Interfaces;
using System.Linq;
using System.Collections.Generic;
using System;
using Accelerider.Windows.Infrastructure;

namespace Accelerider.Windows
{
    public class MainWindowViewModel : ViewModelBase
    {
        private int _transferTaskCount;


        public MainWindowViewModel(IUnityContainer container) : base(container)
        {
            ServicePointManager.DefaultConnectionLimit = 99999;
            GlobalMessageQueue.Enqueue(UiStrings.Message_Welcome);

            EventAggregator.GetEvent<DownloadTaskCreatedEvent>().Subscribe(e => TransferTaskCount++);
            EventAggregator.GetEvent<UploadTaskCreatedEvent>().Subscribe(e => TransferTaskCount++);

            EventAggregator.GetEvent<DownloadTaskEndEvent>().Subscribe(e => TransferTaskCount--);
            EventAggregator.GetEvent<UploadTaskEndEvent>().Subscribe(e => TransferTaskCount--);
        }


        public int TransferTaskCount
        {
            get => _transferTaskCount;
            set => SetProperty(ref _transferTaskCount, value);
        }


        public override void OnLoaded(object view)
        {
            var acceleriderUser = Container.Resolve<IAcceleriderUser>();

            PulishTaskCreatedEvent<DownloadTaskCreatedEvent>(acceleriderUser.GetDownloadingTasks(), OnDownloaded);
            PulishTaskCreatedEvent<DownloadTaskCreatedEvent>(acceleriderUser.GetUploadingTasks(), OnUploaded);
        }

        private void PulishTaskCreatedEvent<T>(IEnumerable<ITransferTaskToken> tokens, EventHandler<TransferTaskStatusChangedEventArgs> handler)
            where T : TaskCreatedEvent, new()
        {
            foreach (var token in tokens)
            {
                token.TransferTaskStatusChanged += handler;
                EventAggregator.GetEvent<T>().Publish(token);
            }
        }

        private void OnUploaded(object sender, TransferTaskStatusChangedEventArgs e)
        {
            if (e.NewStatus != TransferTaskStatusEnum.Completed) return;

            EventAggregator.GetEvent<UploadTaskEndEvent>().Publish(e.Token);
        }

        private void OnDownloaded(object sender, TransferTaskStatusChangedEventArgs e)
        {
            if (e.NewStatus != TransferTaskStatusEnum.Checking) return;

            EventAggregator.GetEvent<DownloadTaskEndEvent>().Publish(e.Token);
        }
    }
}

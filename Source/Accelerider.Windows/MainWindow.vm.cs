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

            EventAggregator.GetEvent<DownloadTaskCreatedEvent>().Subscribe(e => TransferTaskCount += e.Count);
            EventAggregator.GetEvent<UploadTaskCreatedEvent>().Subscribe(e => TransferTaskCount += e.Count);

            EventAggregator.GetEvent<DownloadTaskTranferedEvent>().Subscribe(e => TransferTaskCount--);
            EventAggregator.GetEvent<UploadTaskCompletedEvent>().Subscribe(e => TransferTaskCount--);
        }


        public int TransferTaskCount
        {
            get => _transferTaskCount;
            set => SetProperty(ref _transferTaskCount, value);
        }


        public override void OnLoaded(object view)
        {
            var acceleriderUser =  Container.Resolve<IAcceleriderUser>();

            PulishTaskCreatedEvent<DownloadTaskCreatedEvent>(acceleriderUser.GetDownloadingTasks(), OnDownloaded);
            PulishTaskCreatedEvent<DownloadTaskCreatedEvent>(acceleriderUser.GetUploadingTasks(), OnUploaded);
        }

        private void PulishTaskCreatedEvent<T>(IEnumerable<ITransferTaskToken> tokens, EventHandler<TransferTaskStatusChangedEventArgs> handler)
            where T : TaskCreatedEvent, new()
        {
            EventAggregator.GetEvent<T>().Publish(tokens.Select(token =>
            {
                token.TransferTaskStatusChanged += handler;
                return token;
            }).ToList());
        }

        private void OnUploaded(object sender, TransferTaskStatusChangedEventArgs e)
        {
            if (e.NewStatus != TransferTaskStatusEnum.Completed) return;

            EventAggregator.GetEvent<UploadTaskCompletedEvent>().Publish(e.Token.FileInfo);
        }

        private void OnDownloaded(object sender, TransferTaskStatusChangedEventArgs e)
        {
            if (e.NewStatus != TransferTaskStatusEnum.Checking) return;

            EventAggregator.GetEvent<DownloadTaskTranferedEvent>().Publish(e.Token.FileInfo);
        }
    }
}

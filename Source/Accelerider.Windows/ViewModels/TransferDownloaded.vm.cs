using System;
using System.Collections.Generic;
using Accelerider.Windows.Events;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;
using System.Linq;

namespace Accelerider.Windows.ViewModels
{
    public class TransferDownloadedViewModel : TransferedBaseViewModel<DownloadTaskEndEvent>
    {
        public TransferDownloadedViewModel(IUnityContainer container) : base(container)
        {
        }


        protected override void OnGettingAToken(ITransferTaskToken token)
        {
            base.OnGettingAToken(token);
            token.TransferTaskStatusChanged += OnTransferTaskStatusChanged;
        }

        private void OnTransferTaskStatusChanged(object sender, TransferTaskStatusChangedEventArgs e)
        {
            if (e.OldStatus != TransferTaskStatusEnum.Checking || 
                e.NewStatus != TransferTaskStatusEnum.Completed) return;

            // TODO: Update UI ITransferedFile.FileCheckStatus.
        }

        protected override IReadOnlyCollection<ITransferedFile> GetTransferedFiles() => AcceleriderUser.GetDownloadedFiles();
    }
}

using System.Collections.Generic;
using Accelerider.Windows.Events;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.ViewModels
{
    public class TransferDownloadingViewModel : TransferingBaseViewModel<DownloadTaskCreatedEvent>
    {
        public TransferDownloadingViewModel(IUnityContainer container) : base(container)
        {
        }

        protected override TransferTaskStatusEnum TransferedStatus => TransferTaskStatusEnum.Checking;

        protected override IReadOnlyCollection<ITransferTaskToken> GetInitializedTasks() => AcceleriderUser.GetDownloadingTasks();
    }
}

using System.Collections.Generic;
using Accelerider.Windows.Events;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.ViewModels
{
    public class TransferUploadingViewModel : TransferingBaseViewModel<UploadTaskCreatedEvent>
    {
        public TransferUploadingViewModel(IUnityContainer container) : base(container)
        {
        }

        protected override TransferTaskStatusEnum TransferedStatus => TransferTaskStatusEnum.Completed;

        protected override IReadOnlyCollection<ITransferTaskToken> GetinitializedTasks() => AcceleriderUser.GetUploadingTasks();
    }
}

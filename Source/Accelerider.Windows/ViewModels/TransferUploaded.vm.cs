using System.Collections.Generic;
using Accelerider.Windows.Events;
using Accelerider.Windows.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.ViewModels
{
    public class TransferUploadedViewModel : TransferedBaseViewModel<UploadTaskEndEvent>
    {
        public TransferUploadedViewModel(IUnityContainer container) : base(container)
        {
        }

        protected override IReadOnlyCollection<ITransferedFile> GetTransferedFiles() => AcceleriderUser.GetUploadedFiles();
    }
}

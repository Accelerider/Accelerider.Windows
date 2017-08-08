using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.ViewModels.Items;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.ViewModels
{
    public class TransferDownloadingViewModel : ViewModelBase
    {
        private ObservableCollection<TransferTaskViewModel> _downloadTasks;


        public TransferDownloadingViewModel(IUnityContainer container) : base(container)
        {
            DownloadTasks = new ObservableCollection<TransferTaskViewModel>(AcceleriderUser.GetDownloadingFiles().Select(item =>
            {
                item.TransferStateChanged += OnTransferStateChanged;
                return new TransferTaskViewModel(item);
            }));
        }

        public ObservableCollection<TransferTaskViewModel> DownloadTasks
        {
            get => _downloadTasks;
            set => SetProperty(ref _downloadTasks, value);
        }

        private void OnTransferStateChanged(object sender, TransferStateChangedEventArgs e)
        {
            if (e.NewState == TransferStateEnum.Transfering)
            {
                
            }
            if (e.NewState == TransferStateEnum.Completed)
            {
                
            }
        }
    }
}

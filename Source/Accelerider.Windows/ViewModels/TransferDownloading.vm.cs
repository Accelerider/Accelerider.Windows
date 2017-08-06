using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Accelerider.Windows.ViewModels.Items;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.ViewModels
{
    public class TransferDownloadingViewModel : ViewModelBase
    {
        private ObservableCollection<TransferTaskViewModel> _downloadTasks;


        public TransferDownloadingViewModel(IUnityContainer container) : base(container)
        {
            DownloadTasks = new ObservableCollection<TransferTaskViewModel>(from item in AcceleriderUser.GetDownloadingFiles()
                                                                            select new TransferTaskViewModel(item));
        }

        public ObservableCollection<TransferTaskViewModel> DownloadTasks
        {
            get => _downloadTasks;
            set => SetProperty(ref _downloadTasks, value);
        }
    }
}

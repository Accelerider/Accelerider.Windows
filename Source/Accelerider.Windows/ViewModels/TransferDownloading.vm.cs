using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.ViewModels
{
    public class TransferDownloadingViewModel : ViewModelBase
    {
        private ObservableCollection<ITransferTaskToken> _downloadTasks;


        public ObservableCollection<ITransferTaskToken> DownloadTasks
        {
            get => _downloadTasks;
            set => SetProperty(ref _downloadTasks, value);
        }


        public TransferDownloadingViewModel(IUnityContainer container) : base(container)
        {
        }

    }
}

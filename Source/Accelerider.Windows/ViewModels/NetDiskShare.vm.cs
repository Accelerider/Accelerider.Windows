using Accelerider.Windows.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Accelerider.Windows.ViewModels
{
    public class NetDiskShareViewModel : ViewModelBase
    {
        public NetDiskShareViewModel(IUnityContainer container) : base(container)
        {
        }

        protected override async Task LoadViewModel()
        {
            await Task.Delay(1000);

        }

        private ObservableCollection<ISharedFile> _sharedFiles;

        public ObservableCollection<ISharedFile> sharedFiles
        {
            get { return _sharedFiles; }
            set { SetProperty(ref _sharedFiles, value); }
        }

    }
}

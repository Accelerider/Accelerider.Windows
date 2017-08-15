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

        protected override async Task LoadAsync()
        {
            SharedFiles = new ObservableCollection<ISharedFile>(await NetDiskUser.GetSharedFilesAsync());

        }

        private ObservableCollection<ISharedFile> _sharedFiles;

        public ObservableCollection<ISharedFile> SharedFiles
        {
            get => _sharedFiles;
            set => SetProperty(ref _sharedFiles, value);
        }

    }
}

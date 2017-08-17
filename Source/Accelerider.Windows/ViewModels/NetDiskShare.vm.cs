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

        public override async void OnLoaded()
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

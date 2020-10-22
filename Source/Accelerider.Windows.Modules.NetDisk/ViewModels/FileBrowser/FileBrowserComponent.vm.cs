using System.Collections.ObjectModel;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Mvvm;
using Accelerider.Windows.Modules.NetDisk.Models;
using Unity;
using MaterialDesignThemes.Wpf;

namespace Accelerider.Windows.Modules.NetDisk.ViewModels.FileBrowser
{
    public class FileBrowserComponentViewModel : ViewModelBase, IViewLoadedAndUnloadedAware, INotificable
    {
        public FileBrowserComponentViewModel(IUnityContainer container) : base(container)
        {
        }

        public async void OnLoaded()
        {
            await AcceleriderUser.UpdateAsync();
            RaisePropertyChanged(nameof(NetDiskUsers));
        }

        public void OnUnloaded()
        {
        }

        public ObservableCollection<INetDiskUser> NetDiskUsers =>
            new ObservableCollection<INetDiskUser>(AcceleriderUser.GetNetDiskUsers());

        public ISnackbarMessageQueue GlobalMessageQueue { get; set; }
    }
}

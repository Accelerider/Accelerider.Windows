using System.Collections.Generic;
using System.Threading.Tasks;
using Accelerider.Windows.Modules.NetDisk.Enumerations;
using Accelerider.Windows.Modules.NetDisk.Interfaces;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.Modules.NetDisk.ViewModels.FileBrowser
{
    public class ShareViewModel : LoadingFilesBaseViewModel<ISharedFile>
    {
        public ShareViewModel(IUnityContainer container) : base(container)
        {
        }

        protected override async Task<IList<ISharedFile>> GetFilesAsync() => await CurrentNetDiskUser.GetFilesAsync<ISharedFile>(FileCategory.Shared);
    }
}

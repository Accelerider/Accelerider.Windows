using System.Collections.Generic;
using System.Threading.Tasks;
using Accelerider.Windows.Modules.NetDisk.Enumerations;
using Accelerider.Windows.Modules.NetDisk.Interfaces;
using Autofac;


namespace Accelerider.Windows.Modules.NetDisk.ViewModels.FileBrowser
{
    public class ShareViewModel : LoadingFilesBaseViewModel<ISharedFile>
    {
        public ShareViewModel(IContainer container) : base(container)
        {
        }

        protected override async Task<IList<ISharedFile>> GetFilesAsync() => await CurrentNetDiskUser.GetFilesAsync<ISharedFile>(FileCategory.Shared);
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Accelerider.Windows.Modules.NetDisk.Enumerations;
using Accelerider.Windows.Modules.NetDisk.Interfaces;
using Autofac;


namespace Accelerider.Windows.Modules.NetDisk.ViewModels.FileBrowser
{
    public class RecycleBinViewModel : LoadingFilesBaseViewModel<IDeletedFile>
    {
        public RecycleBinViewModel(IContainer container) : base(container)
        {
        }

        protected override async Task<IList<IDeletedFile>> GetFilesAsync() => await CurrentNetDiskUser.GetFilesAsync<IDeletedFile>(FileCategory.RecycleBin);
    }
}

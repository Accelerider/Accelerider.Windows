using System.Collections.Generic;
using System.Threading.Tasks;
using Accelerider.Windows.Modules.NetDisk.Interfaces;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.Modules.NetDisk.ViewModels.FileBrowser
{
    public class RecycleBinViewModel : LoadingFilesBaseViewModel<IDeletedFile>
    {
        public RecycleBinViewModel(IUnityContainer container) : base(container)
        {
        }

        protected override async Task<IEnumerable<IDeletedFile>> GetFilesAsync() => await NetDiskUser.GetDeletedFilesAsync();
    }
}

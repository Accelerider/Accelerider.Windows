using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Accelerider.Windows.Modules.NetDisk.Models;
using Unity;


namespace Accelerider.Windows.Modules.NetDisk.ViewModels.FileBrowser
{
    public class RecycleBinViewModel : LoadingFilesBaseViewModel<IDeletedFile>
    {
        public RecycleBinViewModel(IUnityContainer container) : base(container)
        {
        }

        protected override async Task<IList<IDeletedFile>> GetFilesAsync()
        {
            //return await CurrentNetDiskUser.GetFilesAsync<IDeletedFile>(FileCategory.RecycleBin);
            throw new NotImplementedException();
        }
    }
}

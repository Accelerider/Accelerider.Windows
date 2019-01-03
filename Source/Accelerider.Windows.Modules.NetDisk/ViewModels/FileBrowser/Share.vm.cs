using Unity;


namespace Accelerider.Windows.Modules.NetDisk.ViewModels.FileBrowser
{
    public class ShareViewModel : ViewModelBase
    {
        public ShareViewModel(IUnityContainer container) : base(container)
        {
        }

        //protected override async Task<IList<ISharedFile>> GetFilesAsync()
        //{
        //    //return await CurrentNetDiskUser.GetFilesAsync<ISharedFile>(FileCategory.Shared);
        //    throw new NotImplementedException();
        //}
    }
}

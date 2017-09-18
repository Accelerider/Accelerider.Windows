using System.Collections.Generic;
using Accelerider.Windows.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;
using System.Threading.Tasks;

namespace Accelerider.Windows.ViewModels.NetDisk
{
    public class ShareViewModel : LoadingFilesBaseViewModel<ISharedFile>
    {
        public ShareViewModel(IUnityContainer container) : base(container)
        {
        }

        protected override async Task<IEnumerable<ISharedFile>> GetFilesAsync()
        {
            return await NetDiskUser.GetSharedFilesAsync();
        }

    }
}

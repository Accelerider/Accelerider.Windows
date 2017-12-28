using System.Collections.Generic;
using System.Threading.Tasks;

using Accelerider.Windows.Infrastructure.Interfaces;

using Microsoft.Practices.Unity;

namespace Accelerider.Windows.ViewModels.NetDisk
{
    public class ShareViewModel : LoadingFilesBaseViewModel<ISharedFile>
    {
        public ShareViewModel(IUnityContainer container) : base(container)
        {
        }

        protected override async Task<IEnumerable<ISharedFile>> GetFilesAsync() => await NetDiskUser.GetSharedFilesAsync();
    }
}

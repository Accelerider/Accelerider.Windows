using System.Collections.Generic;
using Accelerider.Windows.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Accelerider.Windows.ViewModels
{
    public class NetDiskShareViewModel : LoadingFilesViewModel<ISharedFile>
    {
        public NetDiskShareViewModel(IUnityContainer container) : base(container)
        {
        }

        protected override async Task<IEnumerable<ISharedFile>> GetFilesAsync()
        {
            return await NetDiskUser.GetSharedFilesAsync();
        }

    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.ViewModels
{
    public class NetDiskRecycleBinViewModel : LoadingFilesViewModel<IDeletedFile>
    {
        public NetDiskRecycleBinViewModel(IUnityContainer container) : base(container)
        {
        }


        protected override async Task<IEnumerable<IDeletedFile>> GetFilesAsync()
        {
            return await NetDiskUser.GetDeletedFilesAsync();
        }

    }
}

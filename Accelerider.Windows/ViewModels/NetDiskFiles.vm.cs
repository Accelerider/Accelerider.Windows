using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.ViewModels
{
    public class NetDiskFilesViewModel : ViewModelBase
    {
        public NetDiskFilesViewModel(IUnityContainer container) : base(container)
        {
        }

        protected override async Task LoadViewModel()
        {
            CurrentFolder = await NetDiskUser.GetNetDiskFileTreeAsync();
        }

        private ITreeNodeAsync<INetDiskFile> _currentFolder;
        public ITreeNodeAsync<INetDiskFile> CurrentFolder
        {
            get => _currentFolder;
            set => SetProperty(ref _currentFolder, value);
        }



    }
}

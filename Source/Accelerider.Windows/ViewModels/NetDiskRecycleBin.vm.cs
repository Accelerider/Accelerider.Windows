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
    public class NetDiskRecycleBinViewModel : ViewModelBase
    {
        public NetDiskRecycleBinViewModel(IUnityContainer container) : base(container)
        {
        }

        protected override async Task LoadViewModel()
        {
            DeletedFiles = new ObservableCollection<IDeletedFile>(await NetDiskUser.GetDeletedFilesAsync());
        }


        private ObservableCollection<IDeletedFile> _deletedFiles;
        public ObservableCollection<IDeletedFile> DeletedFiles
        {
            get => _deletedFiles;
            set => SetProperty(ref _deletedFiles, value);
        }

    }
}

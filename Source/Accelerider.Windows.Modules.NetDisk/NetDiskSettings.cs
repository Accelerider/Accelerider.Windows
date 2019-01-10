using Accelerider.Windows.Infrastructure;
using Prism.Mvvm;

namespace Accelerider.Windows.Modules.NetDisk
{
    public class NetDiskSettings : BindableBase
    {
        private FileLocator _downloadDirectory;
        private bool _doNotDisplayDownloadDialog;

        public FileLocator DownloadDirectory
        {
            get => _downloadDirectory;
            set => SetProperty(ref _downloadDirectory, value);
        }

        public bool DoNotDisplayDownloadDialog
        {
            get => _doNotDisplayDownloadDialog;
            set => SetProperty(ref _doNotDisplayDownloadDialog, value);
        }
    }
}

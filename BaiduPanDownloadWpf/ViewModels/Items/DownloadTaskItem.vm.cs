using System.Diagnostics;
using System.IO;
using BaiduPanDownloadWpf.Commands;
using BaiduPanDownloadWpf.Infrastructure;
using Microsoft.Practices.Unity;
using System.Threading.Tasks;
using BaiduPanDownloadWpf.Infrastructure.Interfaces.Files;

namespace BaiduPanDownloadWpf.ViewModels.Items
{
    public abstract class DownloadTaskItemViewModel : ViewModelBase
    {
        private readonly IDiskFile _diskFile;
        private Command _openFolderCommand;

        protected DownloadTaskItemViewModel(IUnityContainer container, IDiskFile diskFile)
            : base(container)
        {
            _diskFile = diskFile;
            OpenFolderCommand = new Command(OpenFolderCommandExecuteAsync, () => File.Exists(FilePath.FullPath));
        }

        public long FileId => _diskFile.FileId;
        public FileTypeEnum FileType => _diskFile.FileType;
        public FileLocation FilePath => _diskFile.FilePath;
        public DataSize? FileSize => new DataSize(_diskFile.FileSize);

        public Command OpenFolderCommand
        {
            get { return _openFolderCommand; }
            set { SetProperty(ref _openFolderCommand, value); }
        }
        private void OpenFolderCommandExecuteAsync()
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start(new ProcessStartInfo("explorer.exe")
                    {
                        Arguments = "/e,/select," + FilePath.FullPath
                    });
                }
                catch
                {
                    // ignored
                }
            });
        }
    }
}

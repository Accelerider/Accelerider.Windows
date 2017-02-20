using System;
using System.Diagnostics;
using System.IO;
using BaiduPanDownloadWpf.Commands;
using BaiduPanDownloadWpf.Infrastructure;
using BaiduPanDownloadWpf.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;
using System.Threading.Tasks;
using BaiduPanDownloadWpf.Infrastructure.Interfaces.Files;

namespace BaiduPanDownloadWpf.ViewModels.Items
{
    internal abstract class DownloadTaskItemViewModel : ViewModelBase
    {
        private readonly IDiskFile _diskFile;
        private Command _openFolderCommand;

        protected DownloadTaskItemViewModel(IUnityContainer container, IDiskFile diskFile)
            : base(container)
        {
            _diskFile = diskFile;
            OpenFolderCommand = new Command(OpenFolderCommandExecuteAsync, () => Directory.Exists(FilePath.FolderPath));
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
                    Process.Start("explorer.exe", FilePath.FolderPath);
                }
                catch { }
            });
        }
    }
}

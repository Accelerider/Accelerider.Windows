using System;
using System.Diagnostics;
using System.IO;
using BaiduPanDownloadWpf.Commands;
using BaiduPanDownloadWpf.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;
using System.Threading.Tasks;
using BaiduPanDownloadWpf.Infrastructure.Interfaces.Files;

namespace BaiduPanDownloadWpf.ViewModels.Items
{
    internal class DownloadedTaskItemViewModel : DownloadTaskItemViewModel
    {
        private Command _openFileCommand;
        private Command _clearRecordCommand;

        public DownloadedTaskItemViewModel(IUnityContainer container, ILocalDiskFile diskFile)
            : base(container, diskFile)
        {
            CompletedTime = diskFile.CompletedTime.ToString("yyyy-MM-dd HH:mm");
            OpenFileCommand = new Command(OpenFileCommandExecuteAsync, () => File.Exists(FilePath.FullPath));
            ClearRecordCommand = new Command(ClearRecordCommandExecute);
        }

        public string CompletedTime { get; }

        public Command OpenFileCommand
        {
            get { return _openFileCommand; }
            set { SetProperty(ref _openFileCommand, value); }
        }
        public Command ClearRecordCommand
        {
            get { return _clearRecordCommand; }
            set { SetProperty(ref _clearRecordCommand, value); }
        }

        private void ClearRecordCommandExecute()
        {
            // TODO
            Debug.WriteLine($"{DateTime.Now} : Clear Record Command: {FilePath.FullPath}");
        }
        private void OpenFileCommandExecuteAsync()
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start(new ProcessStartInfo { FileName = FilePath.FullPath });
                }
                catch { }
            });
        }
    }
}

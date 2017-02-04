using Microsoft.Practices.Unity;
using System;
using BaiduPanDownloadWpf.Commands;
using BaiduPanDownloadWpf.Infrastructure.Interfaces;
using System.Windows.Forms;

namespace BaiduPanDownloadWpf.ViewModels.Settings
{
    internal class ConfigurePageViewModel : ViewModelBase
    {
        private readonly ILocalDiskUser _localDiskUser;
        private Command _openFolderBrowserCommand;


        public ConfigurePageViewModel(IUnityContainer container, ILocalDiskUserRepository localDiskUserRepository)
            : base(container)
        {
            _localDiskUser = localDiskUserRepository.FirstOrDefault();
            OpenFolderBrowserCommand = new Command(OpenFolderBrowserCommandExecute, () => _localDiskUser != null);
        }

        public string DownloadPath
        {
            get { return _localDiskUser?.DownloadDirectory; }
            set
            {
                if (_localDiskUser == null) return;
                var temp = _localDiskUser.DownloadDirectory;
                SetProperty(ref temp, value);
                _localDiskUser.DownloadDirectory = temp;
            }
        }

        public int DownloadSpeedLimit
        {
            get { return _localDiskUser?.DownloadThreadNumber ?? 0; }
            set
            {
                if (_localDiskUser == null) return;
                var temp = _localDiskUser.DownloadThreadNumber;
                SetProperty(ref temp, value);
                _localDiskUser.DownloadThreadNumber = temp;
            }
        }
        public int ParallelTaskNumber
        {
            get { return _localDiskUser?.ParallelTaskNumber ?? 0; }
            set
            {
                if (_localDiskUser == null) return;
                var temp = _localDiskUser.ParallelTaskNumber;
                SetProperty(ref temp, value);
                _localDiskUser.ParallelTaskNumber = temp;
            }
        }


        public Command OpenFolderBrowserCommand
        {
            get { return _openFolderBrowserCommand; }
            set { SetProperty(ref _openFolderBrowserCommand, value); }
        }

        private void OpenFolderBrowserCommandExecute()
        {
            var dialog = new FolderBrowserDialog();
            dialog.Description = "Please select the download path";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                DownloadPath = dialog.SelectedPath;
            }
        }
    }
}

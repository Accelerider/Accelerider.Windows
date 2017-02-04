using Microsoft.Practices.Unity;
using System;
using BaiduPanDownloadWpf.Commands;
using BaiduPanDownloadWpf.Infrastructure.Interfaces;
using System.Windows.Forms;

namespace BaiduPanDownloadWpf.ViewModels.Settings
{
    internal class ConfigurePageViewModel : ViewModelBase
    {
        private readonly ILocalDiskUserRepository _localDiskUserRepository;
        private ILocalDiskUser _localDiskUser;
        private Command _openFolderBrowserCommand;


        public ConfigurePageViewModel(IUnityContainer container, ILocalDiskUserRepository localDiskUserRepository)
            : base(container)
        {
            _localDiskUserRepository = localDiskUserRepository;
            OpenFolderBrowserCommand = new Command(OpenFolderBrowserCommandExecute, () => _localDiskUser != null);
        }

        protected override void OnLoaded()
        {
            if (_localDiskUser == null) _localDiskUser = _localDiskUserRepository.FirstOrDefault();
        }

        public string DownloadPath
        {
            get { return _localDiskUser?.DownloadDirectory; }
            set
            {
                if (_localDiskUser == null) return;
                _localDiskUser.DownloadDirectory = value;
                OnPropertyChanged(() => DownloadPath);
            }
        }
        public int DownloadSpeedLimit
        {
            get { return _localDiskUser?.DownloadThreadNumber ?? 0; }
            set
            {
                if (_localDiskUser == null) return;
                _localDiskUser.DownloadThreadNumber = value;
                OnPropertyChanged(() => DownloadSpeedLimit);
            }
        }
        public int ParallelTaskNumber
        {
            get { return _localDiskUser?.ParallelTaskNumber ?? 0; }
            set
            {
                if (_localDiskUser == null) return;
                _localDiskUser.ParallelTaskNumber = value;
                OnPropertyChanged(() => ParallelTaskNumber);
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

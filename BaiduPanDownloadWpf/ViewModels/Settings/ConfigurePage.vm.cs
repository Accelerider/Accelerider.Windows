using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using BaiduPanDownloadWpf.Commands;
using BaiduPanDownloadWpf.Infrastructure.Interfaces;
using System.Windows.Forms;
using BaiduPanDownloadWpf.Assets;
using Prism.Unity;

namespace BaiduPanDownloadWpf.ViewModels.Settings
{
    internal class ConfigurePageViewModel : ViewModelBase
    {
        private readonly IMountUserRepository _mountUserRepository;
        private readonly ILocalConfigInfo _localConfigInfo;
        private IMountUser _localDiskUser;
        private Command _openFolderBrowserCommand;


        public ConfigurePageViewModel(IUnityContainer container, IMountUserRepository mountUserRepository)
            : base(container)
        {
            _localConfigInfo = Container.Resolve<ILocalConfigInfo>();
            _mountUserRepository = mountUserRepository;
            OpenFolderBrowserCommand = new Command(OpenFolderBrowserCommandExecute, () => _localDiskUser != null);
        }

        protected override void OnLoaded()
        {
            if (SetProperty(ref _localDiskUser, _mountUserRepository?.FirstOrDefault())) UpdataAllProperties();
        }

        public string DownloadPath
        {
            get { return _localConfigInfo?.DownloadDirectory; }
            set
            {
                if (_localDiskUser == null) return;
                _localConfigInfo.DownloadDirectory = value;
                OnPropertyChanged(() => DownloadPath);
            }
        }
        public double DownloadSpeedLimit
        {
            get { return _localConfigInfo?.SpeedLimit ?? 0; }
            set
            {
                if (_localDiskUser == null) return;
                _localConfigInfo.SpeedLimit = value;
                OnPropertyChanged(() => DownloadSpeedLimit);
            }
        }
        public int ParallelTaskNumber
        {
            get { return _localConfigInfo?.ParallelTaskNumber ?? 0; }
            set
            {
                if (_localDiskUser == null) return;
                _localConfigInfo.ParallelTaskNumber = value;
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
            var dialog = new FolderBrowserDialog { Description = UiStringResources.Please_select_the_download_path };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                DownloadPath = dialog.SelectedPath;
            }
        }

        private void UpdateAllProperties()
        {
            OnPropertyChanged(() => DownloadPath);
            OnPropertyChanged(() => DownloadSpeedLimit);
            OnPropertyChanged(() => ParallelTaskNumber);
        }

        private void UpdataAllProperties()
        {
            OnPropertyChanged(() => DownloadPath);
            OnPropertyChanged(() => DownloadSpeedLimit);
            OnPropertyChanged(() => ParallelTaskNumber);
        }
    }
}

using System.Windows.Forms;
using System.Windows.Input;
using BaiduPanDownloadWpf.Assets;
using BaiduPanDownloadWpf.Commands;
using BaiduPanDownloadWpf.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;

namespace BaiduPanDownloadWpf.ViewModels.Settings
{
    public class DownloadOptionAreaViewModel : ViewModelBase
    {
        private readonly IMountUserRepository _mountUserRepository;
        private readonly ILocalConfigInfo _localConfigInfo;
        private IMountUser _mountUser;

        public DownloadOptionAreaViewModel(IUnityContainer container, IMountUserRepository mountUserRepository, ILocalConfigInfo localConfigInfo) : base(container)
        {
            _mountUserRepository = mountUserRepository;
            _localConfigInfo = localConfigInfo;
            OpenFolderBrowserCommand = new Command(OpenFolderBrowserCommandExecute, () => _mountUser != null);
        }

        protected override void OnLoaded()
        {
            if (SetProperty(ref _mountUser, _mountUserRepository?.FirstOrDefault())) UpdateLocalConfigInfo();
        }

        public string DownloadPath
        {
            get { return _localConfigInfo?.DownloadDirectory; }
            set
            {
                if (_mountUser == null) return;
                _localConfigInfo.DownloadDirectory = value;
                OnPropertyChanged(() => DownloadPath);
            }
        }
        public double DownloadSpeedLimit
        {
            get { return _localConfigInfo?.SpeedLimit ?? 0; }
            set
            {
                if (_mountUser == null) return;
                _localConfigInfo.SpeedLimit = value;
                OnPropertyChanged(() => DownloadSpeedLimit);
            }
        }
        public int ParallelTaskNumber
        {
            get { return _localConfigInfo?.ParallelTaskNumber ?? 0; }
            set
            {
                if (_mountUser == null) return;
                _localConfigInfo.ParallelTaskNumber = value;
                OnPropertyChanged(() => ParallelTaskNumber);
            }
        }

        private ICommand _openFolderBrowserCommand;
        public ICommand OpenFolderBrowserCommand
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

        private void UpdateLocalConfigInfo()
        {
            OnPropertyChanged(() => DownloadPath);
            OnPropertyChanged(() => DownloadSpeedLimit);
            OnPropertyChanged(() => ParallelTaskNumber);
        }
    }
}

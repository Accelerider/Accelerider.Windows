using System;
using Microsoft.Practices.Unity;
using BaiduPanDownloadWpf.Infrastructure.Interfaces;
using BaiduPanDownloadWpf.Commands;
using System.Windows.Input;
using System.Windows.Forms;
using BaiduPanDownloadWpf.Assets;
using Prism.Logging;
using System.Collections.ObjectModel;
using BaiduPanDownloadWpf.Views.Dialogs;

namespace BaiduPanDownloadWpf.ViewModels.Settings
{
    internal class OptionPageViewModel : ViewModelBase
    {
        private readonly IMountUserRepository _mountUserRepository;
        private readonly ILocalConfigInfo _localConfigInfo;
        private IMountUser _mountUser;



        public OptionPageViewModel(IUnityContainer container, IMountUserRepository mountUserRepository,
            ILocalConfigInfo localConfigInfo) : base(container)
        {
            _mountUserRepository = mountUserRepository;
            _localConfigInfo = localConfigInfo;

            SignOutCommand = new Command(SignOutCommandExecute);
            //BindBaiduAccountCommand = new Command(BindBaiduAccountCommandExecute);

            OpenFolderBrowserCommand = new Command(OpenFolderBrowserCommandExecute, () => _mountUser != null);
        }



        protected override async void OnLoaded()
        {
            if (!SetProperty(ref _mountUser, _mountUserRepository?.FirstOrDefault())) return;

            OnPropertyChanged(() => MountUsername);
            UpdateLocalConfigInfo();
            try
            {
                var netDiskUsers = _mountUser.GetAllNetDiskUsers();
                NetDiskUsers = new ObservableCollection<INetDiskUser>();
                foreach (var item in netDiskUsers)
                {
                    await item.UpdateAsync();
                    NetDiskUsers.Add(item);
                }
            }
            catch (NullReferenceException exception)
            {
                new MessageDialog(UiStringResources.MessageDialogTitle_Error, exception.Message).ShowDialog();
            }
        }

        private ObservableCollection<INetDiskUser> _netDiskUsers;

        public ObservableCollection<INetDiskUser> NetDiskUsers
        {
            get { return _netDiskUsers; }
            set { SetProperty(ref _netDiskUsers, value); }
        }
        public string MountUsername => _mountUser?.Username;


        private ICommand _signOutCommand;
        public ICommand SignOutCommand
        {
            get { return _signOutCommand; }
            set { SetProperty(ref _signOutCommand, value); }
        }
        private void SignOutCommandExecute()
        {
            // 1.Save data.
            if (!_mountUser.IsRememberPassword) _mountUserRepository.Remove(_mountUser.Username);
            _mountUser.SignOut();
            _mountUser.IsAutoSignIn = false;
            _mountUserRepository.Save();
            // 2.Shows the sign window.
            new SignWindow().Show();
            Logger.Log("SignWindow has been created and displayed.", Category.Info, Priority.Low);
            // 3.Closes the SignIn window.
            (System.Windows.Application.Current.Resources[MainWindow.Key] as MainWindow)?.Close();
            Logger.Log("MainWindow has been closed.", Category.Info, Priority.Low);
            Logger.Log($"User: \"{_mountUser.Username}\" has exited.", Category.Info, Priority.Low);
        }

        #region Configure local information
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
        #endregion
    }
}

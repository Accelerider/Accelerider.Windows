using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using BaiduPanDownloadWpf.Assets;
using BaiduPanDownloadWpf.Commands;
using BaiduPanDownloadWpf.Infrastructure.Interfaces;
using BaiduPanDownloadWpf.Views.Dialogs;
using Microsoft.Practices.Unity;
using Prism.Logging;

namespace BaiduPanDownloadWpf.ViewModels.Settings
{
    public class AccountOptionAreaViewModel:ViewModelBase
    {
        private readonly IMountUserRepository _mountUserRepository;
        private IMountUser _mountUser;
        private ObservableCollection<INetDiskUser> _netDiskUsers;
        private ICommand _signOutCommand;


        public AccountOptionAreaViewModel(IUnityContainer container, IMountUserRepository mountUserRepository) : base(container)
        {
            _mountUserRepository = mountUserRepository;
            SignOutCommand = new Command(SignOutCommandExecute);
        }

        protected override async void OnLoaded()
        {
            if (!SetProperty(ref _mountUser, _mountUserRepository?.FirstOrDefault())) return;

            OnPropertyChanged(() => MountUsername);
            try
            {
                var netDiskUsers = _mountUser.GetAllNetDiskUsers();
                NetDiskUsers = new ObservableCollection<INetDiskUser>();
                foreach (var item in netDiskUsers)
                {
                    await item.UpdateAsync();
                    NetDiskUsers.Add(item);
                    Logger.Log($"Net-Disk user \"{item.Username}\" has been added.", Category.Info, Priority.Low);
                }
            }
            catch (NullReferenceException exception)
            {
                new MessageDialog(UiStringResources.MessageDialogTitle_Error, exception.Message).ShowDialog();
            }
        }


        public ObservableCollection<INetDiskUser> NetDiskUsers
        {
            get { return _netDiskUsers; }
            set { SetProperty(ref _netDiskUsers, value); }
        }
        public string MountUsername => _mountUser?.Username;
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
    }
}

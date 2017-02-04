using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using System.Collections.ObjectModel;
using BaiduPanDownloadWpf.Infrastructure.Interfaces;
using BaiduPanDownloadWpf.Commands;
using System.Windows.Controls;

namespace BaiduPanDownloadWpf.ViewModels.Settings
{
    internal class LoginPageViewModel : ViewModelBase
    {
        private ObservableCollection<INetDiskUser> _netDiskUsers;
        private bool _isLoginServiceAccount;
        private bool _isAddingBaiduAccount;
        private string _localDiskUserName;
        private bool _isRememberPassword;
        private bool _isAutoSignIn;
        private bool _isSigningIn;

        private Command<PasswordBox> _loginServiceAccountCommand;
        private Command _signOutServiceAccountCommand;
        private Command _loginBaiduAccountCommand;

        public LoginPageViewModel(IUnityContainer container) : base(container)
        {
            LoginServiceAccountCommand = new Command<PasswordBox>(LoginServiceAccountCommandExecute);
            SignOutServiceAccountCommand = new Command(SignOutServiceAccountCommandExecute);
            LoginBaiduAccountCommand = new Command(LoginBaiduAccountCommandExecute);
        }

        public bool IsLoginServiceAccount
        {
            get { return _isLoginServiceAccount; }
            set { SetProperty(ref _isLoginServiceAccount, value); }
        }
        public bool IsAddingBaiduAccount
        {
            get { return _isAddingBaiduAccount; }
            set { SetProperty(ref _isAddingBaiduAccount, value); }
        }
        public bool IsSigningIn
        {
            get { return _isSigningIn; }
            set { SetProperty(ref _isSigningIn, value); }
        }
        public string LocalDiskUserName
        {
            get { return _localDiskUserName; }
            set { SetProperty(ref _localDiskUserName, value); }
        }

        // 1. If requires automatic login, it is necessary to remember the password.
        // 2. If not requires to remember the password, it also does not require automatic login.
        public bool IsRememberPassword
        {
            get { return _isRememberPassword; }
            set { if (SetProperty(ref _isRememberPassword, value) && !value) IsAutoSignIn = false; }
        }
        public bool IsAutoSignIn
        {
            get { return _isAutoSignIn; }
            set { if (SetProperty(ref _isAutoSignIn, value) && value) IsRememberPassword = true; }
        }
        public ObservableCollection<INetDiskUser> NetDiskUsers
        {
            get { return _netDiskUsers; }
            set { SetProperty(ref _netDiskUsers, value); }
        }

        public Command<PasswordBox> LoginServiceAccountCommand
        {
            get { return _loginServiceAccountCommand; }
            set { SetProperty(ref _loginServiceAccountCommand, value); }
        }
        public Command SignOutServiceAccountCommand
        {
            get { return _signOutServiceAccountCommand; }
            set { SetProperty(ref _signOutServiceAccountCommand, value); }
        }
        public Command LoginBaiduAccountCommand
        {
            get { return _loginBaiduAccountCommand; }
            set { SetProperty(ref _loginBaiduAccountCommand, value); }
        }

        private async void LoginServiceAccountCommandExecute(PasswordBox password)
        {
            IsSigningIn = true;
            var localDiskUserRepository = Container.Resolve<ILocalDiskUserRepository>();
            try
            {
                var localDiskUser = await localDiskUserRepository.SignInAsync(LocalDiskUserName, password.Password);
                localDiskUser.IsRememberPassword = IsRememberPassword;
                localDiskUser.IsAutoSignIn = IsAutoSignIn;
                var netDiskUsers = await localDiskUser.GetAllNetDiskUsers();
                NetDiskUsers = new ObservableCollection<INetDiskUser>();
                foreach (var item in netDiskUsers)
                {
                    NetDiskUsers.Add(item);
                }
                IsLoginServiceAccount = true;
            }
            catch (Exception)
            {

            }
            IsSigningIn = false;
        }
        private void SignOutServiceAccountCommandExecute()
        {
            IsLoginServiceAccount = false;
            IsAddingBaiduAccount = false;
        }
        private void LoginBaiduAccountCommandExecute()
        {

        }
    }
}

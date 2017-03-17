using Microsoft.Practices.Unity;
using System.Collections.ObjectModel;
using BaiduPanDownloadWpf.Infrastructure.Interfaces;
using BaiduPanDownloadWpf.Commands;
using System.Windows.Controls;
using BaiduPanDownloadWpf.Assets;
using BaiduPanDownloadWpf.Infrastructure.Exceptions;
using BaiduPanDownloadWpf.Views.Dialogs;
using Prism.Logging;

namespace BaiduPanDownloadWpf.ViewModels.Settings
{
    internal class LoginPageViewModel : ViewModelBase
    {
        private readonly IMountUserRepository _mountUserRepository;

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
            _mountUserRepository = Container.Resolve<IMountUserRepository>();

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
            try
            {
                var mountUser = await _mountUserRepository.CreateAsync(LocalDiskUserName, password.Password);
                mountUser.IsRememberPassword = IsRememberPassword;
                mountUser.IsAutoSignIn = IsAutoSignIn;
                var netDiskUsers = mountUser.GetAllNetDiskUsers();
                NetDiskUsers = new ObservableCollection<INetDiskUser>();
                foreach (var item in netDiskUsers)
                {
                    NetDiskUsers.Add(item);
                }
                IsLoginServiceAccount = true;
            }
            catch (LoginException loginEx)
            {
                Logger.Log($"Login exception: {loginEx.Message}", Category.Warn, Priority.Low);
                new MessageDialog(UiStringResources.MessageDialogTitle_LoginException, loginEx.Message).ShowDialog();
            }
            catch (System.Runtime.Remoting.ServerException serverEx)
            {
                Logger.Log($"Server exception: {serverEx.Message}", Category.Exception, Priority.High);
                new MessageDialog(UiStringResources.MessageDialogTitle_LoginException, serverEx.Message).ShowDialog();
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

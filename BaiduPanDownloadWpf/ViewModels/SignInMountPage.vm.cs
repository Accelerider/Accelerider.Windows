using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BaiduPanDownloadWpf.Commands;
using Microsoft.Practices.Unity;
using BaiduPanDownloadWpf.Infrastructure.Interfaces;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BaiduPanDownloadWpf.Infrastructure.Exceptions;
using BaiduPanDownloadWpf.Views.Dialogs;
using Prism.Logging;
using BaiduPanDownloadWpf.Assets;
using BaiduPanDownloadWpf.Core;
using BaiduPanDownloadWpf.Views;

namespace BaiduPanDownloadWpf.ViewModels
{
    internal class SignInMountPageViewModel : ViewModelBase
    {
        private readonly IMountUserRepository _mountUserRepository;

        public SignInMountPageViewModel(IUnityContainer container) : base(container)
        {
            _mountUserRepository = Container.Resolve<IMountUserRepository>();
            SignInCommand = new Command<PasswordBox>(SignInCommandExecute,
                p => !(string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(p.Password)));
        }

        protected override void OnLoaded(ContentControl view)
        {
            var users = _mountUserRepository.GetAll();
            if (MountUserCollection == null) MountUserCollection = new ObservableCollection<IMountUser>();
            foreach (var item in users)
            {
                MountUserCollection.Add(item);
            }
            var passwordBox = (view as SignInMountPage)?.PasswordBox;
            if (FillAccontPageInfo(passwordBox, users.FirstOrDefault(item => item.IsAutoSignIn)))
            {
                IsAutoSignIn = true;
                if (SignInCommand.CanExecute(passwordBox)) SignInCommand.Execute(passwordBox);
                return;
            }
            FillAccontPageInfo(passwordBox, users.FirstOrDefault(item => item.IsRememberPassword));
        }

        private string _username;
        private ObservableCollection<IMountUser> _mountUserCollection;
        private bool _isSigningIn;
        private bool _isRememberPassword;
        private bool _isAutoSignIn;

        public string Username
        {
            get { return _username; }
            set { SetProperty(ref _username, value); }
        }
        public ObservableCollection<IMountUser> MountUserCollection
        {
            get { return _mountUserCollection; }
            set { SetProperty(ref _mountUserCollection, value); }
        }
        public bool IsSigningIn
        {
            get { return _isSigningIn; }
            set { SetProperty(ref _isSigningIn, value); }
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


        private ICommand _signInCommand;
        public ICommand SignInCommand
        {
            get { return _signInCommand; }
            set { SetProperty(ref _signInCommand, value); }
        }
        private async void SignInCommandExecute(PasswordBox password)
        {
            // 1.
            IsSigningIn = true;
            try
            {
                // TODO: If the server implements decryption function, the following code will be used.
                var mountUser = await _mountUserRepository.CreateAsync(Username,
                    password.Password.Length <= 117 ? password.Password.ToMd5().EncryptByRSA() : password.Password); // 117 is the maximum allowable encryption length of RSA (1024).
                mountUser.IsRememberPassword = IsRememberPassword;
                mountUser.IsAutoSignIn = IsAutoSignIn;
                Logger.Log($"User: \"{Username}\" has successfully logged in.", Category.Info, Priority.Low);
                // 2.Shows the main window.
                new MainWindow().Show();
                Logger.Log("MainWindow has been created and displayed.", Category.Info, Priority.Low);
                // 3.Closes the SignIn window.
                (Application.Current.Resources[SignWindow.Key] as SignWindow)?.Close();
                Logger.Log("SignWindow has been closed.", Category.Info, Priority.Low);
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
            finally
            {
                IsSigningIn = false;
            }
        }


        private bool FillAccontPageInfo(PasswordBox passwordBox, IMountUser user)
        {
            if (user == null) return false;
            IsRememberPassword = true;
            Username = user.Username;
            passwordBox.Password = user.PasswordEncrypted;
            return true;
        }
    }
}

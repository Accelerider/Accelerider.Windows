using Accelerider.Windows.Commands;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;
using Accelerider.Windows.Views;
using Accelerider.Windows.Views.Dialogs;
using MaterialDesignThemes.Wpf;
using Microsoft.Practices.Unity;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Accelerider.Windows.Events;

namespace Accelerider.Windows.ViewModels
{
    public class SignInViewModel : ViewModelBase
    {
        private string _username;
        private bool _isRememberPassword;
        private bool _isAutoSignIn;

        private ICommand _signInCommand;


        public SignInViewModel(IUnityContainer container) : base(container)
        {
            LocalConfigureInfo = Container.Resolve<ILocalConfigureInfo>();
            SignInCommand = new RelayCommand<PasswordBox>(SignInCommandExecute, passwordBox => !string.IsNullOrEmpty(passwordBox.Password) && !string.IsNullOrEmpty(Username));
        }

        public override void OnLoaded(object view)
        {
            var password = (view as SignInView).PasswordBox;

            if (!string.IsNullOrEmpty(LocalConfigureInfo.Username) &&
                !string.IsNullOrEmpty(LocalConfigureInfo.PasswordEncrypted))
            {
                IsRememberPassword = true;
                Username = LocalConfigureInfo.Username;
                password.Password = LocalConfigureInfo.PasswordEncrypted;
            }
            if (LocalConfigureInfo.IsAutoSignIn &&
                SignInCommand.CanExecute(password))
            {
                IsAutoSignIn = true;
                SignInCommand.Execute(password);
            }
        }

        protected ILocalConfigureInfo LocalConfigureInfo { get; }

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public bool IsRememberPassword
        {
            get => _isRememberPassword;
            set { if (SetProperty(ref _isRememberPassword, value) && !value) IsAutoSignIn = false; }
        }

        public bool IsAutoSignIn
        {
            get => _isAutoSignIn;
            set { if (SetProperty(ref _isAutoSignIn, value) && value) IsRememberPassword = true; }
        }

        public ICommand SignInCommand
        {
            get => _signInCommand;
            set => SetProperty(ref _signInCommand, value);
        }


        private async void SignInCommandExecute(PasswordBox password)
        {
            var passwordMd5 = password.Password.ToMd5();

            EventAggregator.GetEvent<IsLoadingMainWindowEvent>().Publish(true);
            var message = await AcceleriderUser.SignInAsync(Username, passwordMd5 /*.EncryptByRijndael()*/);
            if (!string.IsNullOrEmpty(message))
            {
                GlobalMessageQueue.Enqueue(message, true);
                EventAggregator.GetEvent<IsLoadingMainWindowEvent>().Publish(false);
                LocalConfigureInfo.IsAutoSignIn = false;
                return;
            }

            // Saves data.
            await Task.Run(() =>
            {
                LocalConfigureInfo.PasswordEncrypted = IsRememberPassword ? passwordMd5 : string.Empty;
                LocalConfigureInfo.Username = IsRememberPassword ? Username : string.Empty;
                LocalConfigureInfo.IsAutoSignIn = IsAutoSignIn;
                LocalConfigureInfo.Save();
            });

            // Launches main window and closes itself.
            new MainWindow().Show();
            (Application.Current.Resources[EnteringWindow.Key] as EnteringWindow)?.Close();
            EventAggregator.GetEvent<IsLoadingMainWindowEvent>().Publish(false);
        }
    }
}

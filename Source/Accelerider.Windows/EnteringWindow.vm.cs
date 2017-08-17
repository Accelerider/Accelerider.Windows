using System.Windows.Input;
using Accelerider.Windows.ViewModels;
using Microsoft.Practices.Unity;
using System.Windows;
using System.Windows.Controls;
using Accelerider.Windows.Commands;
using Accelerider.Windows.Infrastructure.Interfaces;
using Accelerider.Windows.Infrastructure;
using System.Threading.Tasks;
using MaterialDesignThemes.Wpf;
using Accelerider.Windows.Views.Dialogs;

namespace Accelerider.Windows
{
    public class EnteringWindowViewModel : ViewModelBase
    {
        private string _username;
        private bool _isRememberPassword;
        private bool _isAutoSignIn;

        private ICommand _onLoadedCommand;
        private ICommand _signInCommand;


        public EnteringWindowViewModel(IUnityContainer container) : base(container)
        {
            MessageQueue = new SnackbarMessageQueue();
            LocalConfigureInfo = Container.Resolve<ILocalConfigureInfo>();
            OnLoadedCommand = new RelayCommand<PasswordBox>(OnLoadedCommandExecute);
            SignInCommand = new RelayCommand<PasswordBox>(SignInCommandExecute);
        }


        protected ILocalConfigureInfo LocalConfigureInfo { get; }

        public string Username
        {
            get { return _username; }
            set { SetProperty(ref _username, value); }
        }

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


        private ISnackbarMessageQueue _messageQueue;

        public ISnackbarMessageQueue MessageQueue
        {
            get { return _messageQueue; }
            set { SetProperty(ref _messageQueue, value); }
        }



        public ICommand OnLoadedCommand
        {
            get { return _onLoadedCommand; }
            set { SetProperty(ref _onLoadedCommand, value); }
        }

        public ICommand SignInCommand
        {
            get => _signInCommand;
            set => SetProperty(ref _signInCommand, value);
        }


        private void OnLoadedCommandExecute(PasswordBox password)
        {
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

        private async void SignInCommandExecute(PasswordBox password)
        {
            var passwordMd5 = password.Password.ToMd5();

            var message = await DialogHost.Show(new WaitingDialog(), "EnteringDialog", async (object sender, DialogOpenedEventArgs e) =>
                e.Session.Close(await AcceleriderUser.SignInAsync(Username, passwordMd5/*.EncryptByRijndael()*/))) as string;
            if (!string.IsNullOrEmpty(message))
            {
                MessageQueue.Enqueue(message, true);
                return;
            }

            // Saves data.
            await Task.Run(() =>
            {
                LocalConfigureInfo.PasswordEncrypted = IsRememberPassword ? passwordMd5.EncryptByRSA() : string.Empty;
                LocalConfigureInfo.Username = IsRememberPassword ? Username : string.Empty;
                LocalConfigureInfo.IsAutoSignIn = IsAutoSignIn;
                LocalConfigureInfo.Save();
            });

            // Launches main window and closes itself.
            new MainWindow().Show();
            (Application.Current.Resources[EnteringWindow.Key] as EnteringWindow)?.Close();
        }
    }
}

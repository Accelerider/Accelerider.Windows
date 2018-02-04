using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Accelerider.Windows.Infrastructure.Commands;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;
using Accelerider.Windows.Models;
using Accelerider.Windows.Views;
using Accelerider.Windows.Views.Entering;
using Microsoft.Practices.Unity;
using Prism.Modularity;

namespace Accelerider.Windows.ViewModels.Entering
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
            SignInCommand = new RelayCommand<PasswordBox>(SignInCommandExecute, passwordBox => CanSignIn(Username, passwordBox.Password));
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


        public override void OnLoaded(object view)
        {
            var password = ((SignInView)view).PasswordBox;

            if (!CanSignIn(LocalConfigureInfo.Username, LocalConfigureInfo.PasswordEncrypted)) return;

            IsRememberPassword = true;
            IsAutoSignIn = LocalConfigureInfo.IsAutoSignIn;
            Username = LocalConfigureInfo.Username;
            password.Password = LocalConfigureInfo.PasswordEncrypted;

            if (IsAutoSignIn)
            {
                SignInCommand.Execute(password);
            }
        }

        private async void SignInCommandExecute(PasswordBox password)
        {
            var passwordEncrypted = password.Password == LocalConfigureInfo.PasswordEncrypted
                ? password.Password
                : password.Password.ToMd5();

            await SignInAsync(Username, passwordEncrypted);
        }

        private async Task SignInAsync(string username, string passwordEncrypted)
        {
            EventAggregator.GetEvent<MainWindowLoadingEvent>().Publish(true);
            var message = await AcceleriderUser.SignInAsync(username, passwordEncrypted);
            if (!string.IsNullOrEmpty(message))
            {
                GlobalMessageQueue.Enqueue(message, true);
                EventAggregator.GetEvent<MainWindowLoadingEvent>().Publish(false);
                LocalConfigureInfo.IsAutoSignIn = false;
                return;
            }
            //EventAggregator.GetEvent<MainWindowLoadingEvent>().Publish(true);
            //if (!await AuthenticateAsync(username, passwordEncrypted))
            //{
            //    EventAggregator.GetEvent<MainWindowLoadingEvent>().Publish(false);
            //    LocalConfigureInfo.IsAutoSignIn = false;
            //    return;
            //}

            var moduleCatalog = Container.Resolve<IModuleCatalog>();
            new ModuleResolver(moduleCatalog).Initialize();
            Container.Resolve<IModuleManager>().Run();

            // Saves data.
            LocalConfigureInfo.Username = IsRememberPassword ? username : string.Empty;
            LocalConfigureInfo.PasswordEncrypted = IsRememberPassword ? passwordEncrypted : string.Empty;
            LocalConfigureInfo.IsAutoSignIn = IsAutoSignIn;
            LocalConfigureInfo.Save();

            // Launches main window and closes itself.
            ShellSwitcher.Switch<EnteringWindow, MainWindow>();
        }

        private async Task<bool> Authenticate(string username, string passwordEncrypted)
        {
            var message = await AcceleriderUser.SignInAsync(username, passwordEncrypted);
            if (string.IsNullOrEmpty(message)) return true;
            GlobalMessageQueue.Enqueue(message, true);
            LocalConfigureInfo.IsAutoSignIn = false;
            return false;
        }

        private bool CanSignIn(string username, string password) => !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password);
    }
}

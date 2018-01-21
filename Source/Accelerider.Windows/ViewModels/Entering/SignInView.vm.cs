using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
using Refit;

namespace Accelerider.Windows.ViewModels.Entering
{
    public class SignInViewModel : ViewModelBase
    {
        private readonly INonAuthenticationApi _nonAuthenticationApi;

        private string _username;
        private bool _isRememberPassword;
        private bool _isAutoSignIn;
        private ICommand _signInCommand;


        public SignInViewModel(IUnityContainer container) : base(container)
        {
            _nonAuthenticationApi = Container.Resolve<INonAuthenticationApi>();
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
            password.Password = LocalConfigureInfo.PasswordEncrypted.DecryptByRijndael();

            if (IsAutoSignIn)
            {
                SignInCommand.Execute(password);
            }
        }

        private async void SignInCommandExecute(PasswordBox password)
        {
            var passwordMd5 = password.Password == LocalConfigureInfo.PasswordEncrypted.DecryptByRijndael()
                            ? password.Password
                            : password.Password.ToMd5();

            await SignInAsync(Username, passwordMd5);
        }

        private async Task SignInAsync(string username, string passwordMd5)
        {
            EventAggregator.GetEvent<MainWindowLoadingEvent>().Publish(true);

            if (!await AuthenticateAsync(username, passwordMd5))
            {
                EventAggregator.GetEvent<MainWindowLoadingEvent>().Publish(false);
                LocalConfigureInfo.IsAutoSignIn = false;
                return;
            }

            await Container.Resolve<ModuleResolver>().LoadAsync();

            // Saves data.
            LocalConfigureInfo.Username = IsRememberPassword ? username : string.Empty;
            LocalConfigureInfo.PasswordEncrypted = IsRememberPassword ? passwordMd5.EncryptByRijndael() : string.Empty;
            LocalConfigureInfo.IsAutoSignIn = IsAutoSignIn;
            LocalConfigureInfo.Save();

            // Launches main window and closes itself.
            ShellSwitcher.Switch<EnteringWindow, MainWindow>();
        }

        private async Task<bool> AuthenticateAsync(string username, string passwordMd5)
        {

            var token = await _nonAuthenticationApi.LoginAsync(new LoginInfoBody
            {
                Username = username,
                Password = passwordMd5.EncryptByRsa()
            }).RunApi();

            if (token == null) return false;

            var acceleriderApi = RestService.For<IAcceleriderApi>(new HttpClient(new ConfigureHeadersHttpClientHandler(token))
            {
                BaseAddress = new Uri(ConstStrings.BaseAddress)
            });

            var user = await acceleriderApi.GetCurrentUserAsync().RunApi();

            if (user == null) return false;

            Container.RegisterInstance(user);
            Container.RegisterInstance(acceleriderApi);

            return true;
        }

        private bool CanSignIn(string username, string password) => !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password);
    }
}

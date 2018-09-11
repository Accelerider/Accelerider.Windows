using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Accelerider.Windows.Constants;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Commands;
using Accelerider.Windows.Infrastructure.Extensions;
using Accelerider.Windows.Infrastructure.Interfaces;
using Accelerider.Windows.Infrastructure.ViewModels;
using Accelerider.Windows.Models;
using Accelerider.Windows.Views;
using Accelerider.Windows.Views.Authentication;
using Autofac;
using Refit;
using SignInView = Accelerider.Windows.Views.Authentication.SignInView;

namespace Accelerider.Windows.ViewModels.Authentication
{
    public class SignInViewModel : ViewModelBase, IAwareViewLoadedAndUnloaded
    {
        private readonly INonAuthenticationApi _nonAuthenticationApi;

        private SignUpInfoBody _signUpInfo;
        private string _email;
        private bool _isRememberPassword;
        private bool _isAutoSignIn;
        private ICommand _signInCommand;


        public SignInViewModel(IContainer container) : base(container)
        {
            _nonAuthenticationApi = Container.Resolve<INonAuthenticationApi>();
            ConfigureFile = Container.Resolve<IConfigureFile>();
            SignInCommand = new RelayCommand<PasswordBox>(SignInCommandExecute, passwordBox => CanSignIn(Email, passwordBox.Password));

            EventAggregator.GetEvent<SignUpSuccessEvent>().Subscribe(signUpInfo => _signUpInfo = signUpInfo);
        }


        protected IConfigureFile ConfigureFile { get; }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
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


        public void OnLoaded(object view)
        {
            var passwordBox = ((SignInView)view).PasswordBox;

            // 1. Login info from SignUpView
            if (_signUpInfo != null)
            {
                IsRememberPassword = false;
                IsAutoSignIn = false;
                Email = _signUpInfo.Username;
                passwordBox.Password = _signUpInfo.Password;

                SignInCommand.Execute(passwordBox);
                _signUpInfo = null;
                return;
            }

            // 2. If there is some residual information on username or password text box, no login information is loaded from elsewhere.
            if (!string.IsNullOrEmpty(Email) || !string.IsNullOrEmpty(passwordBox.Password)) return;

            // 3. No login info from config file.
            if (!CanSignIn(ConfigureFile.GetValue<string>(ConfigureKeys.Username), ConfigureFile.GetValue<string>(ConfigureKeys.Password))) return;

            // 4. Login info from config file.
            IsRememberPassword = true;
            IsAutoSignIn = ConfigureFile.GetValue<bool>(ConfigureKeys.AutoSignIn);
            Email = ConfigureFile.GetValue<string>(ConfigureKeys.Username);
            passwordBox.Password = ConfigureFile.GetValue<string>(ConfigureKeys.Password).DecryptByRijndael();

            if (IsAutoSignIn)
            {
                SignInCommand.Execute(passwordBox);
            }
        }

        public void OnUnloaded(object view)
        {
        }

        private async void SignInCommandExecute(PasswordBox password)
        {
            var passwordMd5 = password.Password == ConfigureFile.GetValue<string>(ConfigureKeys.Password).DecryptByRijndael()
                            ? password.Password
                            : password.Password.ToMd5();

            await SignInAsync(Email, passwordMd5);
        }

        private async Task SignInAsync(string username, string passwordMd5)
        {
            EventAggregator.GetEvent<MainWindowLoadingEvent>().Publish(true);

            if (!await AuthenticateAsync(username, passwordMd5))
            {
                EventAggregator.GetEvent<MainWindowLoadingEvent>().Publish(false);
                ConfigureFile.SetValue(ConfigureKeys.AutoSignIn, false);
                return;
            }

            //await Container.Resolve<ModuleResolver>().LoadAsync();

            // Saves data.
            ConfigureFile.SetValue(ConfigureKeys.Username, IsRememberPassword ? username : string.Empty);
            ConfigureFile.SetValue(ConfigureKeys.Password, IsRememberPassword ? passwordMd5.EncryptByRijndael() : string.Empty);
            ConfigureFile.SetValue(ConfigureKeys.AutoSignIn, IsAutoSignIn);

            // Launches main window and closes itself.
            ShellSwitcher.Switch<AuthenticationWindow, MainWindow>();
        }

        private async Task<bool> AuthenticateAsync(string username, string passwordMd5)
        {
            var token = await _nonAuthenticationApi.LoginAsync(new LoginInfoBody
            {
                Email = username,
                Password = passwordMd5
            }).RunApi();

            token = token?.GetJsonValue("accessToken");
            if (token == null) return false;

            var acceleriderApi = RestService.For<IAcceleriderApi>(Hyperlinks.ApiBaseAddress, new RefitSettings
            {
                AuthorizationHeaderValueGetter = () => Task.FromResult(token)
            });

            var user = await acceleriderApi.GetCurrentUserAsync().RunApi();

            if (user == null) return false;

            RegisterInstance(user, acceleriderApi);

            return true;
        }

        private void RegisterInstance(AcceleriderUser user, IAcceleriderApi acceleriderApi)
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(user).As<IAcceleriderUser>();
            builder.RegisterInstance(acceleriderApi).As<IAcceleriderApi>();
            builder.Update(Container);
        }


        private static bool CanSignIn(string username, string password) => !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password);
    }
}

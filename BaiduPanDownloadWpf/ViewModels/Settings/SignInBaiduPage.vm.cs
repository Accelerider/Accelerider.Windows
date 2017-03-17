using Microsoft.Practices.Unity;
using BaiduPanDownloadWpf.Core;
using Prism.Commands;
using BaiduPanDownloadWpf.Commands;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BaiduPanDownloadWpf.ViewModels.Settings
{
    internal class SignInBaiduPageViewModel : ViewModelBase
    {
        private readonly BaiduServer _baiduServer = new BaiduServer();

        public SignInBaiduPageViewModel(IUnityContainer container) : base(container)
        {
            LoginCommand = new DelegateCommand<PasswordBox>(LoginCommandExecute, p => !string.IsNullOrEmpty(Username) && (_baiduServer.HasVerificationCode ? !string.IsNullOrEmpty(VCode) : true))
                .ObservesProperty(() => Username).ObservesProperty(() => VCode);
            UpdateVerificationCodeCommand = new Command(UpdateVerificationCodeCommandExecute);
        }

        private string _username;
        public string Username
        {
            get { return _username; }
            set { if (SetProperty(ref _username, value)) OnUsernameChanged(value); }
        }

        private string _vCode;
        public string VCode
        {
            get { return _vCode; }
            set { if (SetProperty(ref _vCode, value)) _baiduServer.SetVerificationCode(value); }
        }
        public ImageSource VCodeImage => _baiduServer.VerificationCodeImageUri == null ? null : new BitmapImage(_baiduServer.VerificationCodeImageUri);

        private ICommand _loginCommand;
        private ICommand _updateVerificationCodeCommand;

        public ICommand LoginCommand
        {
            get { return _loginCommand; }
            set { SetProperty(ref _loginCommand, value); }
        }
        public ICommand UpdateVerificationCodeCommand
        {
            get { return _updateVerificationCodeCommand; }
            set { SetProperty(ref _updateVerificationCodeCommand, value); }
        }

        private async void UpdateVerificationCodeCommandExecute()
        {
            await _baiduServer.UpdateVerificationCodeAsync();
            OnPropertyChanged(() => VCodeImage);
        }
        private async void LoginCommandExecute(PasswordBox obj)
        {
            await _baiduServer.SetPasswordAsync(obj.Password);
            await _baiduServer.LoginAsync();
        }

        private async void OnUsernameChanged(string username)
        {
            await _baiduServer.SetUsernameAsync(username);
            OnPropertyChanged(() => VCodeImage);
        }
    }
}

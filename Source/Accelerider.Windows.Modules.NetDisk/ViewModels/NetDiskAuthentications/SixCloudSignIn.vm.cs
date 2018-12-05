using System.Windows.Controls;
using System.Windows.Input;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Modules.NetDisk.Models.SixCloud;
using Unity;

namespace Accelerider.Windows.Modules.NetDisk.ViewModels.NetDiskAuthentications
{
    internal class SixCloudSignInViewModel : ViewModelBase
    {

        private string _username;

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public ICommand SignInCommand { get; }

        public SixCloudSignInViewModel(IUnityContainer container) : base(container)
        {
            var user = Container.Resolve<SixCloudUser>();
 
            SignInCommand = new RelayCommand<PasswordBox>(
                async passwordBox =>
                {
                    await user.LoginAsync(Username, passwordBox.Password);
                    await AcceleriderUser.AddNetDiskUserAsync(user);
                },
                passwordBox => !string.IsNullOrWhiteSpace(passwordBox.Password) &&
                               !string.IsNullOrWhiteSpace(Username));
        }
    }
}

using System.Windows.Controls;
using System.Windows.Input;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Modules.NetDisk.Models.SixCloud;
using MaterialDesignThemes.Wpf;
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
            SignInCommand = new RelayCommand<PasswordBox>(
                async passwordBox =>
                {
                    var user = Container.Resolve<SixCloudUser>();
                    if (await user.LoginAsync(Username, passwordBox.Password))
                    {
                        AcceleriderUser.AddNetDiskUser(user);

                        // TODO: Close the view
                    }
                },
                passwordBox => !string.IsNullOrWhiteSpace(passwordBox.Password) &&
                               !string.IsNullOrWhiteSpace(Username));
        }
    }
}

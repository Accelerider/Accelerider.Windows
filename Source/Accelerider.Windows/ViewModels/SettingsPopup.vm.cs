using System.Windows.Input;
using Accelerider.Windows.Commands;
using Accelerider.Windows.Common;
using Accelerider.Windows.Infrastructure.Interfaces;
using Accelerider.Windows.Views;
using Accelerider.Windows.Views.Entering;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.ViewModels
{
    public class SettingsPopupViewModel : ViewModelBase
    {

        private ICommand _changeProfileCommand;
        private ICommand _openOtherSettingsCommand;
        private ICommand _helpCommand;
        private ICommand _checkUpdateCommand;
        private ICommand _openOfficialSiteCommand;
        private ICommand _signOutCommand;


        public SettingsPopupViewModel(IUnityContainer container) : base(container)
        {
            SignOutCommand = new RelayCommand(() =>
            {
                Container.Resolve<ILocalConfigureInfo>().IsAutoSignIn = false;
                ShellController.Switch<MainWindow, EnteringWindow>();
            });
        }


        public ICommand ChangeProfileCommand
        {
            get => _changeProfileCommand;
            set => SetProperty(ref _changeProfileCommand, value);
        }

        public ICommand OpenOtherSettingsCommand
        {
            get => _openOtherSettingsCommand;
            set => SetProperty(ref _openOtherSettingsCommand, value);
        }

        public ICommand HelpCommand
        {
            get => _helpCommand;
            set => SetProperty(ref _helpCommand, value);
        }

        public ICommand CheckUpdateCommand
        {
            get => _checkUpdateCommand;
            set => SetProperty(ref _checkUpdateCommand, value);
        }

        public ICommand OpenOfficialSiteCommand
        {
            get => _openOfficialSiteCommand;
            set => SetProperty(ref _openOfficialSiteCommand, value);
        }

        public ICommand SignOutCommand
        {
            get => _signOutCommand;
            set => SetProperty(ref _signOutCommand, value);
        }

    }
}

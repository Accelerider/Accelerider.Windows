using System.Windows.Input;
using Accelerider.Windows.Commands;
using Accelerider.Windows.Common;
using Accelerider.Windows.Infrastructure.Interfaces;
using Accelerider.Windows.Views;
using Accelerider.Windows.Views.Entering;
using Microsoft.Practices.Unity;
using System.Diagnostics;
using MaterialDesignThemes.Wpf;

namespace Accelerider.Windows.ViewModels
{
    public class SettingsPopupViewModel : ViewModelBase
    {
        private const string _helpUrl = "https://github.com/Accelerider/Accelerider.Windows/wiki";
        private const string _releaseUrl = "https://github.com/Accelerider/Accelerider.Windows/releases";
        private const string _webUrl = "http://pan.accelerider.com";
        private const string _aboutUrl = "https://github.com/Accelerider/Accelerider.Windows";


        private ICommand _changeProfileCommand;
        private ICommand _openOtherSettingsCommand;
        private ICommand _helpCommand;
        private ICommand _aboutCommand;
        private ICommand _checkUpdateCommand;
        private ICommand _openOfficialSiteCommand;
        private ICommand _signOutCommand;


        public SettingsPopupViewModel(IUnityContainer container) : base(container)
        {


            HelpCommand = new RelayCommand(() => OpenWebPage(_helpUrl));
            OpenOfficialSiteCommand = new RelayCommand(() => OpenWebPage(_webUrl));
            CheckUpdateCommand = new RelayCommand(() => OpenWebPage(_releaseUrl));
            AboutCommand = new RelayCommand(() => OpenWebPage(_aboutUrl));

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

        // -----------------------------------------------------------------------------------------------------

        public ICommand HelpCommand
        {
            get => _helpCommand;
            set => SetProperty(ref _helpCommand, value);
        }

        public ICommand OpenOfficialSiteCommand
        {
            get => _openOfficialSiteCommand;
            set => SetProperty(ref _openOfficialSiteCommand, value);
        }

        public ICommand CheckUpdateCommand
        {
            get => _checkUpdateCommand;
            set => SetProperty(ref _checkUpdateCommand, value);
        }

        public ICommand AboutCommand
        {
            get => _aboutCommand;
            set => SetProperty(ref _aboutCommand, value);
        }

        // -----------------------------------------------------------------------------------------------------

        public ICommand SignOutCommand
        {
            get => _signOutCommand;
            set => SetProperty(ref _signOutCommand, value);
        }


        private void OpenWebPage(string url) => Process.Start(url);

        private async void OpenDialog(object dialog) => await DialogHost.Show(dialog, "RootDialog");
    }
}

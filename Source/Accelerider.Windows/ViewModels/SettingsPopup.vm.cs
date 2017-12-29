using System.Diagnostics;
using System.Windows.Input;

using Accelerider.Windows.Commands;
using Accelerider.Windows.Common;
using Accelerider.Windows.Infrastructure.Interfaces;
using Accelerider.Windows.Views;
using Accelerider.Windows.Views.Dialogs;
using Accelerider.Windows.Views.Entering;

using MaterialDesignThemes.Wpf;

using Microsoft.Practices.Unity;

namespace Accelerider.Windows.ViewModels
{
    public class SettingsPopupViewModel : ViewModelBase
    {
        private ICommand _changeProfileCommand;
        private ICommand _openSettingsPanelCommand;
        private ICommand _helpCommand;
        private ICommand _aboutCommand;
        private ICommand _checkUpdateCommand;
        private ICommand _openOfficialSiteCommand;
        private ICommand _signOutCommand;

        private SettingsPopup _view;
        private ProfileDialog _profileDialog;
        private SettingsDialog _settingsDialog;
        private AboutDialog _aboutDialog;

        public SettingsPopupViewModel(IUnityContainer container) : base(container)
        {
            ChangeProfileCommand = new RelayCommand(() => OpenDialog(_profileDialog ?? (_profileDialog = new ProfileDialog())));

            OpenSettingsPanelCommand = new RelayCommand(() => OpenDialog(_settingsDialog ?? (_settingsDialog = new SettingsDialog())));

            HelpCommand = new RelayCommand(() => OpenWebPage(ConstStrings.HelpUrl));
            OpenOfficialSiteCommand = new RelayCommand(() => OpenWebPage(ConstStrings.WebSitePanUrl));
            CheckUpdateCommand = new RelayCommand(() => OpenWebPage(ConstStrings.ReleaseUrl));
            AboutCommand = new RelayCommand(() => OpenDialog(_aboutDialog ?? (_aboutDialog = new AboutDialog())));

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

        // -----------------------------------------------------------------------------------------------------
        public ICommand OpenSettingsPanelCommand
        {
            get => _openSettingsPanelCommand;
            set => SetProperty(ref _openSettingsPanelCommand, value);
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

        public override void OnLoaded(object view)
        {
            base.OnLoaded(view);
            _view = view as SettingsPopup;
        }

        private void OpenWebPage(string url) => Process.Start(url);

        private async void OpenDialog(object dialog)
        {
            _view.SetValue(System.Windows.Controls.Primitives.Popup.IsOpenProperty, false);
            await DialogHost.Show(dialog, "RootDialog");
        }
    }
}

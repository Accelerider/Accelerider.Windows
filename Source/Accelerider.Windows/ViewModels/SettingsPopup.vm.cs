using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using Accelerider.Windows.Constants;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.ViewModels;
using Accelerider.Windows.Views;
using Accelerider.Windows.Views.Dialogs;
using Autofac;
using MaterialDesignThemes.Wpf;


namespace Accelerider.Windows.ViewModels
{
    public class SettingsPopupViewModel : ViewModelBase, IAwareViewLoadedAndUnloaded<SettingsPopup>
    {
        private readonly Dictionary<Type, object> _dialogDictionary = new Dictionary<Type, object>();

        private ICommand _changeProfileCommand;
        private ICommand _openSettingsPanelCommand;
        private ICommand _helpCommand;
        private ICommand _aboutCommand;
        private ICommand _openOfficialSiteCommand;
        private ICommand _signOutCommand;

        private SettingsPopup _view;

        public SettingsPopupViewModel(IContainer container) : base(container)
        {
            ChangeProfileCommand = new RelayCommand(OpenDialog<ProfileDialog>);

            OpenSettingsPanelCommand = new RelayCommand(OpenDialog<SettingsDialog>);

            HelpCommand = new RelayCommand(() => Process.Start(Hyperlinks.Help));
            OpenOfficialSiteCommand = new RelayCommand(() => Process.Start(Hyperlinks.WebSite));
            AboutCommand = new RelayCommand(OpenDialog<AboutDialog>);

            SignOutCommand = new RelayCommand(() =>
            {
                Container.Resolve<IConfigureFile>().SetValue(ConfigureKeys.AutoSignIn, false);
                ProcessController.Restart();
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

        public void OnLoaded(SettingsPopup view)
        {
            _view = view;
        }

        public void OnUnloaded(SettingsPopup view)
        {
        }

        private async void OpenDialog<T>() where T : new()
        {
            var type = typeof(T);

            if (!_dialogDictionary.ContainsKey(type))
                _dialogDictionary[type] = new T();

            _view.SetValue(System.Windows.Controls.Primitives.Popup.IsOpenProperty, false);
            await DialogHost.Show(_dialogDictionary[type], "RootDialog");
        }
    }
}

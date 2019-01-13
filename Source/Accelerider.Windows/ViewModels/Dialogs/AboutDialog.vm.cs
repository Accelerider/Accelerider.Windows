using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Accelerider.Windows.Constants;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Mvvm;
using Unity;


namespace Accelerider.Windows.ViewModels.Dialogs
{
    public class AboutDialogViewModel : ViewModelBase
    {
        private ICommand _openReleaseNotesCommand;
        private ICommand _openProjectHomeCommand;
        private ICommand _checkForUpdateCommand;


        public AboutDialogViewModel(IUnityContainer container) : base(container)
        {
            OpenReleaseNotesCommand = new RelayCommand(() => Process.Start(AcceleriderUrls.Release));
            OpenProjectHomeCommand = new RelayCommand(() => Process.Start(AcceleriderUrls.ProjectGithubHome));
            CheckForUpdateCommand = new RelayCommand(CheckForUpdateCommandExecute);
        }

        public string Version => AcceleriderConsts.Version;

        public ICommand OpenReleaseNotesCommand
        {
            get => _openReleaseNotesCommand;
            set => SetProperty(ref _openReleaseNotesCommand, value);
        }

        public ICommand OpenProjectHomeCommand
        {
            get => _openProjectHomeCommand;
            set => SetProperty(ref _openProjectHomeCommand, value);
        }

        public ICommand CheckForUpdateCommand
        {
            get => _checkForUpdateCommand;
            set => SetProperty(ref _checkForUpdateCommand, value);
        }

        private void CheckForUpdateCommandExecute()
        {
        }
    }
}

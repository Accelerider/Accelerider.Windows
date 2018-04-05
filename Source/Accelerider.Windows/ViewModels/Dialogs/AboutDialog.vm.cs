using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Commands;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.ViewModels.Dialogs
{
    public class AboutDialogViewModel : ViewModelBase
    {
        private ICommand _openReleaseNotesCommand;
        private ICommand _openProjectHomeCommand;
        private ICommand _openMrs4sEmailCommand;
        private ICommand _openLd50EmailCommand;
        private ICommand _openMrs4SHomeCommand;
        private ICommand _openLd50HomeCommand;
        private ICommand _checkForUpdateCommand;


        public AboutDialogViewModel(IUnityContainer container) : base(container)
        {
            OpenReleaseNotesCommand = new RelayCommand(() => Process.Start(ConstStrings.ReleaseUrl));
            OpenProjectHomeCommand = new RelayCommand(() => Process.Start(ConstStrings.GithubHomeUrl));
            OpenMrs4sEmailCommand = new RelayCommand(() => Process.Start("mailto:mrs4sxiaoshi@gmail.com"));
            OpenLd50EmailCommand = new RelayCommand(() => Process.Start("mailto:ld50.zhang@gmail.com"));
            OpenMrs4SHomeCommand = new RelayCommand(() => Process.Start("https://github.com/Mrs4s"));
            OpenLd50HomeCommand = new RelayCommand(() => Process.Start("https://github.com/DingpingZhang"));
            CheckForUpdateCommand = new RelayCommand(CheckForUpdateCommandExecute);
        }


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

        public ICommand OpenMrs4sEmailCommand
        {
            get => _openMrs4sEmailCommand;
            set => SetProperty(ref _openMrs4sEmailCommand, value);
        }

        public ICommand OpenLd50EmailCommand
        {
            get => _openLd50EmailCommand;
            set => SetProperty(ref _openLd50EmailCommand, value);
        }

        public ICommand OpenMrs4SHomeCommand
        {
            get => _openMrs4SHomeCommand;
            set => SetProperty(ref _openMrs4SHomeCommand, value);
        }

        public ICommand OpenLd50HomeCommand
        {
            get => _openLd50HomeCommand;
            set => SetProperty(ref _openLd50HomeCommand, value);
        }

        public ICommand CheckForUpdateCommand
        {
            get => _checkForUpdateCommand;
            set => SetProperty(ref _checkForUpdateCommand, value);
        }

        private void CheckForUpdateCommandExecute()
        {
            var process = new Process();
            process.StartInfo.FileName = Path.Combine(Environment.CurrentDirectory, "Update/Accelerider.Windows.Update.exe");
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            Application.Current.Shutdown(0);
        }
    }
}

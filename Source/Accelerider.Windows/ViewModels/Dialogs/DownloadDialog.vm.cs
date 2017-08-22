using System.Windows.Forms;
using System.Windows.Input;
using Accelerider.Windows.Assets;
using Accelerider.Windows.Commands;
using Accelerider.Windows.Infrastructure.Interfaces;
using MaterialDesignThemes.Wpf;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.ViewModels.Dialogs
{
    public class DownloadDialogViewModel : ViewModelBase
    {
        private ICommand _downloadCommand;
        private ICommand _openFolderDialogCommand;
        private bool _notDisplayDownloadDialog;
        private string _toDownloadFileName;
        private string _downloadFolder;


        public DownloadDialogViewModel(IUnityContainer container) : base(container)
        {
            DownloadFolder = Container.Resolve<ILocalConfigureInfo>().DownloadDirectory;

            OpenFolderDialogCommand = new RelayCommand(OpenFolderDialogCommandExecute);
            DownloadCommand = new RelayCommand(
                () =>
                {
                    if (DialogHost.CloseDialogCommand.CanExecute(true, null))
                        DialogHost.CloseDialogCommand.Execute(true, null);
                }, 
                () => !string.IsNullOrEmpty(DownloadFolder));
        }


        public ICommand DownloadCommand
        {
            get => _downloadCommand;
            set => SetProperty(ref _downloadCommand, value);
        }
        public ICommand OpenFolderDialogCommand
        {
            get => _openFolderDialogCommand;
            set => SetProperty(ref _openFolderDialogCommand, value);
        }

        public bool NotDisplayDownloadDialog
        {
            get => _notDisplayDownloadDialog;
            set => SetProperty(ref _notDisplayDownloadDialog, value);
        }
        public string ToDownloadFileName
        {
            get => _toDownloadFileName;
            set => SetProperty(ref _toDownloadFileName, value);
        }
        public string DownloadFolder
        {
            get => _downloadFolder;
            set => SetProperty(ref _downloadFolder, value);
        }


        private void OpenFolderDialogCommandExecute()
        {
            var dialog = new FolderBrowserDialog { Description = UiStrings.DownloadDialog_FolderBrowerDialogDescription };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                DownloadFolder = dialog.SelectedPath;
            }
        }
    }
}

using System.Windows.Forms;
using System.Windows.Input;
using Accelerider.Windows.Assets;
using Accelerider.Windows.Commands;
using Accelerider.Windows.Infrastructure.Interfaces;
using MaterialDesignThemes.Wpf;
using Microsoft.Practices.Unity;
using Accelerider.Windows.Infrastructure;
using System.Collections.ObjectModel;
using System.Linq;

namespace Accelerider.Windows.ViewModels.Dialogs
{
    public class DownloadDialogViewModel : ViewModelBase
    {
        private ICommand _downloadCommand;
        private ICommand _openFolderDialogCommand;
        private bool _notDisplayDownloadDialog;
        private string _downloadItemsSummary;
        private FileLocation _downloadFolder;
        private ObservableCollection<FileLocation> _defaultFolders;


        public DownloadDialogViewModel(IUnityContainer container) : base(container)
        {
            var downloadDirectory = Container.Resolve<ILocalConfigureInfo>().DownloadDirectory;

            DefaultFolders = new ObservableCollection<FileLocation>(SystemFolder.GetAvailableFolders());
            DownloadFolder = string.IsNullOrEmpty(downloadDirectory) ? DefaultFolders.FirstOrDefault() : downloadDirectory;

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

        public string DownloadItemsSummary
        {
            get => _downloadItemsSummary;
            set => SetProperty(ref _downloadItemsSummary, value);
        }

        public FileLocation DownloadFolder
        {
            get => _downloadFolder;
            set => SetProperty(ref _downloadFolder, value);
        }

        public ObservableCollection<FileLocation> DefaultFolders
        {
            get { return _defaultFolders; }
            set { SetProperty(ref _defaultFolders, value); }
        }



        private void OpenFolderDialogCommandExecute()
        {
            var dialog = new FolderBrowserDialog { Description = UiStrings.DownloadDialog_FolderBrowerDialogDescription };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                DownloadFolder = dialog.SelectedPath;
                DefaultFolders.Insert(0, DownloadFolder);
            }
        }
    }
}

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Commands;
using Accelerider.Windows.Infrastructure.Interfaces;
using Accelerider.Windows.Resources.I18N;
using MaterialDesignThemes.Wpf;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.Modules.NetDisk.ViewModels.Dialogs
{
    public class DownloadDialogViewModel : ViewModelBase
    {
        private ICommand _downloadCommand;
        private ICommand _openFolderDialogCommand;
        private bool _notDisplayDownloadDialog;
        private List<string> _downloadItems;
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

        public string DownloadItemsHint => $"Items ({DownloadItems?.Count})"; // Because of unknown reason, materialDesign:HintAssist.Hint="{Binding DownloadItems.Count, StringFormat='Items ({0})'}" don't work.

        public string DownloadItemsSummary => DownloadItems == null ? string.Empty : string.Join("; ", DownloadItems);

        public List<string> DownloadItems
        {
            get => _downloadItems;
            set
            {
                if (SetProperty(ref _downloadItems, value))
                {
                    OnPropertyChanged(nameof(DownloadItemsHint));
                    OnPropertyChanged(nameof(DownloadItemsSummary));
                }
            }
        }

        public FileLocation DownloadFolder
        {
            get => _downloadFolder;
            set => SetProperty(ref _downloadFolder, value);
        }

        public ObservableCollection<FileLocation> DefaultFolders
        {
            get => _defaultFolders;
            set => SetProperty(ref _defaultFolders, value);
        }



        private void OpenFolderDialogCommandExecute()
        {
            var dialog = new FolderBrowserDialog { Description = UiStrings.DownloadDialog_FolderBrowerDialogDescription };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                DownloadFolder = dialog.SelectedPath;
                if (!DefaultFolders.Contains(DownloadFolder)) DefaultFolders.Insert(0, DownloadFolder);
            }
        }
    }
}

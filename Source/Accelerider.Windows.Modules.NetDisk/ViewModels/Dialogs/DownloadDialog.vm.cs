﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Modules.NetDisk.Constants;
using Accelerider.Windows.Resources.I18N;
using Autofac;
using MaterialDesignThemes.Wpf;


namespace Accelerider.Windows.Modules.NetDisk.ViewModels.Dialogs
{
    public class DownloadDialogViewModel : ViewModelBase
    {
        private ICommand _downloadCommand;
        private ICommand _openFolderDialogCommand;
        private bool _notDisplayDownloadDialog;
        private List<string> _downloadItems;
        private FileLocator _downloadFolder;
        private ObservableCollection<FileLocator> _defaultFolders;


        public DownloadDialogViewModel(IContainer container) : base(container)
        {
            var downloadDirectory = Container.Resolve<IConfigureFile>().GetValue<FileLocator>(ConfigureKeys.DownloadDirectory);

            DefaultFolders = new ObservableCollection<FileLocator>(SystemFolder.GetAvailableFolders());
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
                if (!SetProperty(ref _downloadItems, value)) return;

                RaisePropertyChanged(nameof(DownloadItemsHint));
                RaisePropertyChanged(nameof(DownloadItemsSummary));
            }
        }

        public FileLocator DownloadFolder
        {
            get => _downloadFolder;
            set => SetProperty(ref _downloadFolder, value);
        }

        public ObservableCollection<FileLocator> DefaultFolders
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

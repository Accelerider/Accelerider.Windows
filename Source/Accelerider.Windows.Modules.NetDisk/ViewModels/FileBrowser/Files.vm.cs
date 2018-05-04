using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Commands;
using Accelerider.Windows.Infrastructure.Interfaces;
using Accelerider.Windows.Modules.NetDisk.Constants;
using Accelerider.Windows.Modules.NetDisk.Enumerations;
using Accelerider.Windows.Modules.NetDisk.Interfaces;
using Accelerider.Windows.Modules.NetDisk.ViewModels.Dialogs;
using Accelerider.Windows.Modules.NetDisk.ViewModels.Others;
using Accelerider.Windows.Modules.NetDisk.Views.Dialogs;
using Accelerider.Windows.Modules.NetDisk.Views.FileBrowser;
using Accelerider.Windows.Resources.I18N;
using Accelerider.Windows.TransportEngine;
using MaterialDesignThemes.Wpf;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.Modules.NetDisk.ViewModels.FileBrowser
{
    public class FilesViewModel : LoadingFilesBaseViewModel<ILazyTreeNode<INetDiskFile>>
    {
        private readonly TransferringTaskList _downloadList;
        private readonly TransferringTaskList _uploadList;

        private ILazyTreeNode<INetDiskFile> _selectedSearchResult;
        private ILazyTreeNode<INetDiskFile> _currentFolder;

        private ICommand _enterFolderCommand;
        private ICommand _downloadCommand;
        private ICommand _uploadCommand;
        private ICommand _shareCommand;
        private ICommand _deleteCommand;

        public FilesViewModel(IUnityContainer container) : base(container)
        {
            InitializeCommands();

            _downloadList = Container.Resolve<TransferringTaskList>(TransferringTaskList.DownloadKey);
            _uploadList = Container.Resolve<TransferringTaskList>(TransferringTaskList.UploadKey);

            EventAggregator.GetEvent<SelectedSearchResultChangedEvent>().Subscribe(selectedSearchResult => SelectedSearchResult = selectedSearchResult);
        }

        public ILazyTreeNode<INetDiskFile> SelectedSearchResult
        {
            get => _selectedSearchResult;
            set => SetProperty(ref _selectedSearchResult, value);
        }

        public ILazyTreeNode<INetDiskFile> CurrentFolder
        {
            get => _currentFolder;
            set { if (SetProperty(ref _currentFolder, value)) RefreshFiles(); }
        }

        #region Commands
        public ICommand EnterFolderCommand
        {
            get => _enterFolderCommand;
            set => SetProperty(ref _enterFolderCommand, value);
        }
        public ICommand DownloadCommand
        {
            get => _downloadCommand;
            set => SetProperty(ref _downloadCommand, value);
        }
        public ICommand UploadCommand
        {
            get => _uploadCommand;
            set => SetProperty(ref _uploadCommand, value);
        }
        public ICommand ShareCommand
        {
            get => _shareCommand;
            set => SetProperty(ref _shareCommand, value);
        }
        public ICommand DeleteCommand
        {
            get => _deleteCommand;
            set => SetProperty(ref _deleteCommand, value);
        }


        private void InitializeCommands()
        {
            EnterFolderCommand = new RelayCommand<ILazyTreeNode<INetDiskFile>>(file => CurrentFolder = file, file => file?.Content?.FileType == FileType.FolderType);
            DownloadCommand = new RelayCommand<IList>(DownloadCommandExecute, files => files != null && files.Count > 0);
            UploadCommand = new RelayCommand(UploadCommandExecute);
            ShareCommand = new RelayCommand<IList>(ShareCommandExecute, files => files != null && files.Count > 0);
            DeleteCommand = new RelayCommand<IList>(DeleteCommandExecute, files => files != null && files.Count > 0);
        }

        private async void DownloadCommandExecute(IList files)
        {
            var fileArray = files.Cast<ILazyTreeNode<INetDiskFile>>().ToArray();

            var (folder, isDownload) = await DisplayDownloadDialogAsync(fileArray.Select(item => item.Content.FilePath.FileName));

            if (!isDownload) return;

            var tokens = new List<ITaskReference>();
            foreach (var file in fileArray)
            {
                await NetDiskUser.DownloadAsync(file, folder, token =>
                {
                    // Add new task to download list.
                    _downloadList.Add(new TransportingTaskItem(token));
                    // Records tokens
                    tokens.Add(token);
                });
            }

            var fileName = TrimFileName(tokens.First().FileSummary.FilePath.FileName, 40);
            var message = tokens.Count == 1
                ? string.Format(UiStrings.Message_AddedFileToDownloadList, fileName)
                : string.Format(UiStrings.Message_AddedFilesToDownloadList, fileName, tokens.Count);
            GlobalMessageQueue.Enqueue(message);
        }

        private async void UploadCommandExecute()
        {
            var dialog = new System.Windows.Forms.OpenFileDialog { Multiselect = true };
            if (dialog.ShowDialog() != DialogResult.OK || dialog.FileNames.Length <= 0) return;

            var tokens = new List<ITaskReference>();
            await Task.Run(() =>
            {
                foreach (var fromPath in dialog.FileNames)
                {
                    var toPath = CurrentFolder.Content.FilePath;
                    var token = NetDiskUser.UploadAsync(fromPath, toPath);
                    // Add new task to download list.
                    _uploadList.Add(new TransportingTaskItem(token));
                    // Records tokens
                    tokens.Add(token);
                }
            });

            var fileName = TrimFileName(dialog.FileNames[0], 40);
            var message = dialog.FileNames.Length == 1
                ? string.Format(UiStrings.Message_AddedFileToUploadList, fileName)
                : string.Format(UiStrings.Message_AddedFilesToUploadList, fileName, dialog.FileNames.Length);
            GlobalMessageQueue.Enqueue(message);
        }

        private async void ShareCommandExecute(IList files)
        {
            // 1. Display dialog.

            // 2. Determines whether to share based on the return value of dialog.

            var (code, shareSummary) = await NetDiskUser.ShareAsync(files.Cast<ILazyTreeNode<INetDiskFile>>().Select(node => node.Content));

            // 3. Sends the GlobalMessageQueue for reporting result.
        }

        private async void DeleteCommandExecute(IList files)
        {
            var currentFolder = CurrentFolder;
            var fileArray = files.Cast<ILazyTreeNode<INetDiskFile>>().ToArray();

            var errorFileCount = 0;
            foreach (var file in fileArray)
            {
                if (!await file.Content.DeleteAsync()) errorFileCount++;
            }
            if (errorFileCount < fileArray.Length)
            {
                await currentFolder.RefreshChildrenCacheAsync();
                if (currentFolder == CurrentFolder)
                {
                    OnPropertyChanged(nameof(CurrentFolder));
                }
            }
            GlobalMessageQueue.Enqueue($"({fileArray.Length - errorFileCount}/{fileArray.Length}) files have been deleted.");
        }
        #endregion

        protected override async Task<IEnumerable<ILazyTreeNode<INetDiskFile>>> GetFilesAsync()
        {
            if (PreviousNetDiskUser != NetDiskUser)
            {
                PreviousNetDiskUser = NetDiskUser;
                _currentFolder = await NetDiskUser.GetNetDiskFileRootAsync();
                OnPropertyChanged(nameof(CurrentFolder));
            }
            await CurrentFolder.RefreshChildrenCacheAsync();
            return CurrentFolder.ChildrenCache;
        }

        private async void RefreshFiles()
        {
            if (CurrentFolder.ChildrenCache == null)
                await LoadingFilesAsync();
            else
                Files = CurrentFolder.ChildrenCache;

            EventAggregator.GetEvent<SearchResultsChangedEvent>().Publish(Files);
        }

        private async Task<(string folder, bool isDownload)> DisplayDownloadDialogAsync(IEnumerable<string> files)
        {
            var configure = Container.Resolve<IConfigureFile>();
            if (configure.GetValue<bool>(ConfigureKeys.NotDisplayDownloadDialog))
                return (configure.GetValue<string>(ConfigureKeys.DownloadDirectory), true);

            var dialog = new DownloadDialog();
            var vm = dialog.DataContext as DownloadDialogViewModel;
            vm.DownloadItems = files.ToList();

            if (!(bool)await DialogHost.Show(dialog, "RootDialog")) return (null, false);

            configure.SetValue(ConfigureKeys.NotDisplayDownloadDialog, vm.NotDisplayDownloadDialog);
            if (vm.NotDisplayDownloadDialog)
            {
                configure.SetValue(ConfigureKeys.DownloadDirectory, vm.DownloadFolder);
            }
            return (vm.DownloadFolder, true);
        }

        private string TrimFileName(string fileName, int length)
        {
            FileLocation fileLocation = fileName;
            var folderNameLength = length - fileLocation.FileName.Length - 5;
            return fileName.Length > length
                ? folderNameLength > 0
                    ? fileLocation.FolderPath.Substring(0, folderNameLength) + "...\\" + fileLocation.FileName
                    : fileLocation.FileName
                : fileName;
        }

        public override void OnLoaded(object view)
        {
            base.OnLoaded(view);

            var fileView = (Files)view;
            fileView.ListboxFileList.SelectionChanged += OnSelectedFileItemChanged;
        } 

        public override void OnUnloaded(object view)
        {
            base.OnUnloaded(view);

            var fileView = (Files)view;
            fileView.ListboxFileList.SelectionChanged -= OnSelectedFileItemChanged;
        }

        private void OnSelectedFileItemChanged(object sender, SelectionChangedEventArgs e)
        {
            var fileList = (System.Windows.Controls.ListBox)sender;
            fileList.ScrollIntoView(SelectedSearchResult);
        }
    }
}

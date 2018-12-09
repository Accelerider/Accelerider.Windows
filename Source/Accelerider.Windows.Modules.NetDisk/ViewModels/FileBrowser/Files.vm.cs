using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Mvvm;
using Accelerider.Windows.Modules.NetDisk.Constants;
using Accelerider.Windows.Modules.NetDisk.Enumerations;
using Accelerider.Windows.Modules.NetDisk.Interfaces;
using Accelerider.Windows.Modules.NetDisk.Models;
using Accelerider.Windows.Modules.NetDisk.ViewModels.Dialogs;
using Accelerider.Windows.Modules.NetDisk.Views.Dialogs;
using Accelerider.Windows.Modules.NetDisk.Views.FileBrowser;
using Accelerider.Windows.Resources.I18N;
using Unity;
using MaterialDesignThemes.Wpf;


namespace Accelerider.Windows.Modules.NetDisk.ViewModels.FileBrowser
{
    public class FilesViewModel : LoadingFilesBaseViewModel<ILazyTreeNode<INetDiskFile>>, IViewLoadedAndUnloadedAware<Files>, INotificable
    {
        private ILazyTreeNode<INetDiskFile> _selectedSearchResult;
        private ILazyTreeNode<INetDiskFile> _currentFolder;

        public ISnackbarMessageQueue GlobalMessageQueue { get; set; }

        public FilesViewModel(IUnityContainer container) : base(container)
        {
            InitializeCommands();

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

        public ICommand EnterFolderCommand { get; private set; }

        public ICommand DownloadCommand { get; private set; }

        public ICommand UploadCommand { get; private set; }

        public ICommand ShareCommand { get; private set; }

        public ICommand DeleteCommand { get; private set; }


        private void InitializeCommands()
        {
            EnterFolderCommand = new RelayCommand<ILazyTreeNode<INetDiskFile>>(file => CurrentFolder = file, file => file?.Content?.Type == FileType.FolderType);
            DownloadCommand = new RelayCommand<IList>(DownloadCommandExecute, files => files != null && files.Count > 0);
            UploadCommand = new RelayCommand(UploadCommandExecute);
            ShareCommand = new RelayCommand<IList>(ShareCommandExecute, files => files != null && files.Count > 0);
            DeleteCommand = new RelayCommand<IList>(DeleteCommandExecute, files => files != null && files.Count > 0);
        }

        private async void DownloadCommandExecute(IList files)
        {
            var fileArray = files.Cast<ILazyTreeNode<INetDiskFile>>().ToArray();

            (string to, bool isDownload) = await DisplayDownloadDialogAsync(fileArray.Select(item => item.Content.Path.FileName));

            if (!isDownload) return;

            var downloadItemList = fileArray.Select(file => CurrentNetDiskUser.Download(file, to)).ToList();
            downloadItemList.First().Run();
            var fileName = TrimFileName(downloadItemList.First().File.Path.FileName, 40);
            var message = downloadItemList.Count == 1
                ? string.Format(UiStrings.Message_AddedFileToDownloadList, fileName)
                : string.Format(UiStrings.Message_AddedFilesToDownloadList, fileName, downloadItemList.Count);
            GlobalMessageQueue.Enqueue(message);
        }

        private async void UploadCommandExecute()
        {
            var dialog = new OpenFileDialog { Multiselect = true };
            if (dialog.ShowDialog() != DialogResult.OK || dialog.FileNames.Length <= 0) return;

            var uploadItemList = new List<TransferItem>();
            await Task.Run(() =>
            {
                foreach (var from in dialog.FileNames)
                {
                    var to = CurrentFolder.Content;
                    var token = CurrentNetDiskUser.UploadAsync(from, to, item =>
                    {
                        // Add new task to download list. ??

                        // Records tokens
                        uploadItemList.Add(item);
                    });
                }
            });

            var fileName = TrimFileName(dialog.FileNames[0], 40);
            var message = dialog.FileNames.Length == 1
                ? string.Format(UiStrings.Message_AddedFileToUploadList, fileName)
                : string.Format(UiStrings.Message_AddedFilesToUploadList, fileName, dialog.FileNames.Length);
            GlobalMessageQueue.Enqueue(message);
        }

        private void ShareCommandExecute(IList files)
        {
            // 1. Display dialog.

            // 2. Determines whether to share based on the return value of dialog.

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
                await currentFolder.RefreshAsync();
                if (currentFolder == CurrentFolder)
                {
                    RaisePropertyChanged(nameof(CurrentFolder));
                }
            }
            GlobalMessageQueue.Enqueue($"({fileArray.Length - errorFileCount}/{fileArray.Length}) files have been deleted.");
        }
        #endregion

        protected override async Task<IList<ILazyTreeNode<INetDiskFile>>> GetFilesAsync()
        {
            if (PreviousNetDiskUser != CurrentNetDiskUser)
            {
                PreviousNetDiskUser = CurrentNetDiskUser;
                _currentFolder = await CurrentNetDiskUser.GetFileRootAsync();
                RaisePropertyChanged(nameof(CurrentFolder));
            }
            await CurrentFolder.RefreshAsync();
            return CurrentFolder.ChildrenCache?.ToList();
        }

        private async void RefreshFiles()
        {
            await LoadingFilesAsync(CurrentFolder.ChildrenCache?.ToList());

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
            FileLocator fileLocation = fileName;
            var folderNameLength = length - fileLocation.FileName.Length - 5;
            return fileName.Length > length
                ? folderNameLength > 0
                    ? fileLocation.FolderPath.Substring(0, folderNameLength) + "...\\" + fileLocation.FileName
                    : fileLocation.FileName
                : fileName;
        }

        public void OnLoaded(Files view)
        {
            base.OnLoaded();

            view.ListboxFileList.SelectionChanged += OnSelectedFileItemChanged;
        }

        public void OnUnloaded(Files view)
        {
            base.OnUnloaded();

            view.ListboxFileList.SelectionChanged -= OnSelectedFileItemChanged;
        }

        private void OnSelectedFileItemChanged(object sender, SelectionChangedEventArgs e)
        {
            var fileList = (System.Windows.Controls.ListBox)sender;
            fileList.ScrollIntoView(SelectedSearchResult);
        }
    }
}

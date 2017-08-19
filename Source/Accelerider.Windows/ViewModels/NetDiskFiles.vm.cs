using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Accelerider.Windows.Commands;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;
using System.Collections;
using System.Linq;
using Accelerider.Windows.Events;
using Accelerider.Windows.Assets;
using System;
using Accelerider.Windows.Views.Dialogs;
using MaterialDesignThemes.Wpf;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using Accelerider.Windows.ViewModels.Dialogs;

namespace Accelerider.Windows.ViewModels
{
    public class NetDiskFilesViewModel : ViewModelBase
    {
        private ITreeNodeAsync<INetDiskFile> _currentFolder;
        private INetDiskUser _netDiskUser;

        private ICommand _enterFolderCommand;
        private ICommand _refreshChildrenCacheCommand;
        private ICommand _downloadCommand;
        private ICommand _uploadCommand;
        private ICommand _shareCommand;
        private ICommand _deleteCommand;


        public NetDiskFilesViewModel(IUnityContainer container) : base(container)
        {
            InitializeCommands();
        }


        public override async void OnLoaded()
        {
            EventAggregator.GetEvent<CurrentNetDiskUserChangedEvent>().Subscribe(OnCurrentNetDiskUserChanged);
            if (_netDiskUser != NetDiskUser) CurrentFolder = await NetDiskUser.GetNetDiskFileRootAsync();
        }

        public override void OnUnloaded()
        {
            EventAggregator.GetEvent<CurrentNetDiskUserChangedEvent>().Unsubscribe(OnCurrentNetDiskUserChanged);
            _netDiskUser = NetDiskUser;
        }

        private async void OnCurrentNetDiskUserChanged(INetDiskUser currentNetDiskUser)
        {
            var dialog = new WaitingDialog();
            await DialogHost.Show(dialog, "RootDialog", async (object sender, DialogOpenedEventArgs e) =>
            {
                CurrentFolder = await NetDiskUser.GetNetDiskFileRootAsync();
                e.Session.Close();
            });
        }

        public ITreeNodeAsync<INetDiskFile> CurrentFolder
        {
            get => _currentFolder;
            set { if (SetProperty(ref _currentFolder, value) && CurrentFolder.ChildrenCache == null) RefreshChildrenCacheExecute(); }
        }

        #region Command properties
        public ICommand EnterFolderCommand
        {
            get => _enterFolderCommand;
            set => SetProperty(ref _enterFolderCommand, value);
        }
        public ICommand RefreshChildrenCacheCommand
        {
            get => _refreshChildrenCacheCommand;
            set => SetProperty(ref _refreshChildrenCacheCommand, value);
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
        #endregion


        private void InitializeCommands()
        {
            EnterFolderCommand = new RelayCommand<ITreeNodeAsync<INetDiskFile>>(file => CurrentFolder = file, file => file?.Content?.FileType == FileTypeEnum.FolderType);
            RefreshChildrenCacheCommand = new RelayCommand(RefreshChildrenCacheExecute);
            DownloadCommand = new RelayCommand<IList>(DownloadCommandExecute, files => files != null && files.Count > 0);
            UploadCommand = new RelayCommand(UploadCommandExecute);
            ShareCommand = new RelayCommand<IList>(ShareCommandExecute, files => files != null && files.Count > 0);
            DeleteCommand = new RelayCommand<IList>(DeleteCommandExecute, files => files != null && files.Count > 0);
        }

        private void DeleteCommandExecute(IList obj)
        {
            GlobalMessageQueue.Enqueue("throw new NotImplementedException()");
        }

        private async void DeleteCommandExecute(ITreeNodeAsync<INetDiskFile> file)
        {
            var currentFolder = CurrentFolder;
            var result = await file.Content.DeleteAsync();
            if (result)
            {
                //currentFolder.ChildrenCache.Remove(file);
                await currentFolder.TryGetChildrenAsync();
                if (currentFolder == CurrentFolder)
                {
                    OnPropertyChanged(nameof(CurrentFolder));
                }
            }

            var message = result
                ? $"\"{file.Content.FilePath.FileName}\" has been deleted."
                : $"Deletes \"{file.Content.FilePath.FileName}\" file failed.";
            GlobalMessageQueue.Enqueue(message);
        }

        private async void ShareCommandExecute(IList files)
        {
            var netDiskFiles = new Collection<INetDiskFile>();
            foreach (ITreeNodeAsync<INetDiskFile> file in files)
            {
                netDiskFiles.Add(file.Content);
            }
            var (code, _) = await NetDiskUser.ShareAsync(netDiskFiles);
        }

        private async void DownloadCommandExecute(IList files)
        {
            var (folder, isDownload) = await DisplayDownloadDialogAsync(files.Cast<ITreeNodeAsync<INetDiskFile>>()
                .Select(item => item.Content.FilePath.FileName));
            if (!isDownload) return;

            var tokens = new List<ITransferTaskToken>();
            foreach (ITreeNodeAsync<INetDiskFile> file in files)
            {
                tokens.AddRange(await NetDiskUser.DownloadAsync(file, folder));
            }

            PulishDownloadTaskCreatedEvent(tokens);

            var fileName = GetFilePathWithFixedLength(tokens.First().FileInfo.FilePath.FileName, 40);
            var message = tokens.Count == 1
                ? string.Format(UiStrings.Message_AddedFileToDownloadList, fileName)
                : string.Format(UiStrings.Message_AddedFilesToDownloadList, fileName, tokens.Count);
            GlobalMessageQueue.Enqueue(message);
        }


        private async void UploadCommandExecute()
        {
            var dialog = new OpenFileDialog() { Multiselect = true };
            if (dialog.ShowDialog() != DialogResult.OK || dialog.FileNames.Length <= 0) return;

            IEnumerable<ITransferTaskToken> tokens = null;
            await Task.Run(() =>
            {
                tokens = dialog.FileNames.Select(fromPath =>
                {
                    var toPath = CurrentFolder.Content.FilePath;
                    return NetDiskUser.UploadAsync(fromPath, toPath);
                });
            });

            EventAggregator.GetEvent<UploadTaskCreatedEvent>().Publish(tokens.Select(token =>
            {
                token.TransferStateChanged += OnUploaded;
                return token;
            }).ToList());

            var fileName = GetFilePathWithFixedLength(dialog.FileNames[0], 40);
            var message = dialog.FileNames.Length == 1
                ? string.Format(UiStrings.Message_AddedFileToUploadList, fileName)
                : string.Format(UiStrings.Message_AddedFilesToUploadList, fileName, dialog.FileNames.Length);
            GlobalMessageQueue.Enqueue(message);
        }

        private async void RefreshChildrenCacheExecute()
        {
            await CurrentFolder.TryGetChildrenAsync();
            OnPropertyChanged(nameof(CurrentFolder));
        }


        private async Task<(string folder, bool isDownload)> DisplayDownloadDialogAsync(IEnumerable<string> files)
        {
            var configure = Container.Resolve<ILocalConfigureInfo>();
            if (configure.NotDisplayDownloadDialog) return (null, true);

            var dialog = new DownloadDialog();
            var vm = dialog.DataContext as DownloadDialogViewModel;
            vm.ToDownloadFileName = string.Join("; ", files);

            if (!(bool)await DialogHost.Show(dialog, "RootDialog")) return (null, false);

            if (configure.NotDisplayDownloadDialog = vm.NotDisplayDownloadDialog)
            {
                configure.DownloadDirectory = vm.DownloadFolder;
            }
            return (vm.ToDownloadFileName, true);
        }

        private void PulishDownloadTaskCreatedEvent(IEnumerable<ITransferTaskToken> tokens)
        {
            EventAggregator.GetEvent<DownloadTaskCreatedEvent>().Publish(tokens.Select(token =>
            {
                token.TransferStateChanged += OnDownloaded;
                return token;
            }).ToList());
        }

        private void OnUploaded(object sender, TransferStateChangedEventArgs e)
        {
            if (e.NewState != TransferStateEnum.Checking) return; // TODO: 

            GlobalMessageQueue.Enqueue($"\"{e.Token.FileInfo.FilePath.FileName}\" ({e.Token.FileInfo.FileSize}) has been uploaded.");
            EventAggregator.GetEvent<UploadTaskCompletedEvent>().Publish(e.Token.FileInfo);
        }

        private void OnDownloaded(object sender, TransferStateChangedEventArgs e)
        {
            if (e.NewState != TransferStateEnum.Checking) return;

            GlobalMessageQueue.Enqueue($"\"{e.Token.FileInfo.FilePath.FileName}\" ({e.Token.FileInfo.FileSize}) has been downloaded.");
            EventAggregator.GetEvent<DownloadTaskTranferedEvent>().Publish(e.Token.FileInfo);
        }

        private string GetFilePathWithFixedLength(string filePath, int length)
        {
            FileLocation fileLocation = filePath;
            var folderNameLength = length - fileLocation.FileName.Length - 5;
            return filePath.Length > length
                ? folderNameLength > 0
                    ? fileLocation.FolderPath.Substring(0, folderNameLength) + "...\\" + fileLocation.FileName
                    : fileLocation.FileName
                : filePath;
        }
    }
}

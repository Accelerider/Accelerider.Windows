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

namespace Accelerider.Windows.ViewModels
{
    public class NetDiskFilesViewModel : ViewModelBase
    {
        private ITreeNodeAsync<INetDiskFile> _currentFolder;
        private INetDiskUser _netDiskUser;

        private ICommand _enterFolderCommand;
        private ICommand _refreshChildrenCacheCommand;
        private ICommand _downloadCommand;
        private ICommand _downloadBatchCommand;
        private ICommand _uploadCommand;
        private ICommand _shareCommand;
        private ICommand _shareBatchCommand;
        private ICommand _deleteCommand;
        private ICommand _deleteBatchCommand;


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
            set { if (SetProperty(ref _currentFolder, value) && CurrentFolder.ChildrenCache == null) RefreshChildrenCacheAsync(); }
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
        public ICommand DownloadBatchCommand
        {
            get => _downloadBatchCommand;
            set => SetProperty(ref _downloadBatchCommand, value);
        }
        public ICommand UploadCommand
        {
            get { return _uploadCommand; }
            set { SetProperty(ref _uploadCommand, value); }
        }
        public ICommand ShareCommand
        {
            get { return _shareCommand; }
            set { SetProperty(ref _shareCommand, value); }
        }
        public ICommand ShareBatchCommand
        {
            get { return _shareBatchCommand; }
            set { SetProperty(ref _shareBatchCommand, value); }
        }
        public ICommand DeleteCommand
        {
            get { return _deleteCommand; }
            set { SetProperty(ref _deleteCommand, value); }
        }
        public ICommand DeleteBatchCommand
        {
            get { return _deleteBatchCommand; }
            set { SetProperty(ref _deleteBatchCommand, value); }
        }
        #endregion


        private void InitializeCommands()
        {
            EnterFolderCommand = new RelayCommand<ITreeNodeAsync<INetDiskFile>>(
                file => CurrentFolder = file,
                file => file?.Content?.FileType == FileTypeEnum.FolderType);
            RefreshChildrenCacheCommand = new RelayCommand(RefreshChildrenCacheAsync);
            DownloadCommand = new RelayCommand<ITreeNodeAsync<INetDiskFile>>(DownloadCommandExecute);
            DownloadBatchCommand = new RelayCommand<IList>(files =>
            {
                foreach (var file in files)
                {
                    if (DownloadCommand.CanExecute(file)) DownloadCommand.Execute(file);
                }
            }, files => files != null && files.Count > 0);
            UploadCommand = new RelayCommand(UploadCommandExecute);
        }

        private async void UploadCommandExecute()
        {
            var dialog = new OpenFileDialog() { Multiselect = true };
            if (dialog.ShowDialog() != DialogResult.OK || dialog.FileNames.Length <= 0) return;
            await Task.Run(() =>
            {
                foreach (var fromPath in dialog.FileNames)
                {
                    var toPath = CurrentFolder.Content.FilePath;
                    var token = NetDiskUser.UploadAsync(fromPath, toPath);

                    EventAggregator.GetEvent<UploadTaskCreatedEvent>().Publish(token);
                }
                var fileName = GetFilePathWithFixedLength(dialog.FileNames[0], 40);

                var message = dialog.FileNames.Length == 1
                    ? string.Format(UiStrings.Message_AddedFileToUploadList, fileName)
                    : string.Format(UiStrings.Message_AddedFilesToUploadList, fileName, dialog.FileNames.Length);
                GlobalMessageQueue.Enqueue(message);
            });
        }

        private async void DownloadCommandExecute(ITreeNodeAsync<INetDiskFile> fileNode)
        {
            var tokens = await NetDiskUser.DownloadAsync(fileNode);

            EventAggregator.GetEvent<DownloadTaskCreatedEvent>().Publish(tokens);
            GlobalMessageQueue.Enqueue(
                    fileNode.Content.FileType == FileTypeEnum.FolderType
                    ? string.Format(UiStrings.Message_AddedFolderToDownloadList, fileNode.Content.FilePath.FileName, tokens.Count)
                    : string.Format(UiStrings.Message_AddedFileToDownloadList, fileNode.Content.FilePath.FileName));
        }

        private async void RefreshChildrenCacheAsync()
        {
            await CurrentFolder.TryGetChildrenAsync();
            OnPropertyChanged(nameof(CurrentFolder));
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

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

namespace Accelerider.Windows.ViewModels
{
    public class NetDiskFilesViewModel : ViewModelBase
    {
        private ITreeNodeAsync<INetDiskFile> _currentFolder;

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


        protected override async Task LoadAsync()
        {
            await base.LoadAsync();
            CurrentFolder = await NetDiskUser.GetNetDiskFileTreeAsync();
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
            // 1.gets the the path of local files. to call winform method.
            var fromPath = "";
            var toPath = CurrentFolder.Content.FilePath;
            var token = await NetDiskUser.UploadAsync(fromPath, toPath);

            EventAggregator.GetEvent<UploadTaskCreatedEvent>().Publish(token);
            GlobalMessageQueue.Enqueue(string.Format(UiStrings.Message_AddedFileToUploadList, token.FileInfo.FilePath.FileName));
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

    }
}

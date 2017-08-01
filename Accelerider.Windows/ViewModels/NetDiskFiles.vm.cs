using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Accelerider.Windows.Commands;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;
using MaterialDesignThemes.Wpf;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.ViewModels
{
    public class NetDiskFilesViewModel : ViewModelBase
    {
        private SnackbarMessageQueue _messageQueue;
        private ITreeNodeAsync<INetDiskFile> _currentFolder;
        private ICommand _enterFolderCommand;
        private ICommand _updateChildrenCacheCommand;
        private ICommand _downloadCommand;


        public NetDiskFilesViewModel(IUnityContainer container) : base(container)
        {
            MessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(1));

            EnterFolderCommand = new RelayCommand<ITreeNodeAsync<INetDiskFile>>(
                file => CurrentFolder = file,
                file => file?.Content?.FileType == FileTypeEnum.FolderType
                );
            UpdateChildrenCacheCommand = new RelayCommand(UpdateChildrenCacheAsync);
            DownloadCommand = new RelayCommand<ITreeNodeAsync<INetDiskFile>>(DownloadCommandExecute);
        }

        protected override async Task LoadViewModel()
        {
            CurrentFolder = await NetDiskUser.GetNetDiskFileTreeAsync();
        }

        public SnackbarMessageQueue MessageQueue
        {
            get => _messageQueue;
            set => SetProperty(ref _messageQueue, value);
        }

        public ITreeNodeAsync<INetDiskFile> CurrentFolder
        {
            get => _currentFolder;
            set
            {
                if (EqualityComparer<ITreeNodeAsync<INetDiskFile>>.Default.Equals(_currentFolder, value)) return;
                _currentFolder = value;
                if (CurrentFolder.ChildrenCache == null)
                    UpdateChildrenCacheAsync();
                else
                    OnPropertyChanged(nameof(CurrentFolder));
            }
        }

        public ICommand EnterFolderCommand
        {
            get => _enterFolderCommand;
            set => SetProperty(ref _enterFolderCommand, value);
        }

        public ICommand UpdateChildrenCacheCommand
        {
            get => _updateChildrenCacheCommand;
            set => SetProperty(ref _updateChildrenCacheCommand, value);
        }

        public ICommand DownloadCommand
        {
            get => _downloadCommand;
            set => SetProperty(ref _downloadCommand, value);
        }


        private void DownloadCommandExecute(ITreeNodeAsync<INetDiskFile> fileNode)
        {
            MessageQueue.Enqueue($"Added {fileNode.Content.FilePath.FileName} to Download list.");
        }

        private async void UpdateChildrenCacheAsync()
        {
            await CurrentFolder.TryGetChildrenAsync();
            OnPropertyChanged(nameof(CurrentFolder));
        }

    }
}

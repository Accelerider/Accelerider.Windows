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

namespace Accelerider.Windows.ViewModels
{
    public class NetDiskFilesViewModel : ViewModelBase
    {
        private ITreeNodeAsync<INetDiskFile> _currentFolder;
        private ICommand _enterFolderCommand;
        private ICommand _updateChildrenCacheCommand;
        private ICommand _downloadCommand;
        private ICommand _downloadsCommand;


        public NetDiskFilesViewModel(IUnityContainer container) : base(container)
        {
            EnterFolderCommand = new RelayCommand<ITreeNodeAsync<INetDiskFile>>(
                file => CurrentFolder = file,
                file => file?.Content?.FileType == FileTypeEnum.FolderType
                );
            UpdateChildrenCacheCommand = new RelayCommand(UpdateChildrenCacheAsync);
            DownloadCommand = new RelayCommand<ITreeNodeAsync<INetDiskFile>>(DownloadCommandExecute);
            DownloadsCommand = new RelayCommand<IList>(files =>
            {
                foreach (var file in files)
                {
                    if (DownloadCommand.CanExecute(file)) DownloadCommand.Execute(file);
                }
            });
        }

        protected override async Task Load()
        {
            CurrentFolder = await NetDiskUser.GetNetDiskFileTreeAsync();
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

        public ICommand DownloadsCommand
        {
            get => _downloadsCommand;
            set => SetProperty(ref _downloadsCommand, value);
        }


        private async void DownloadCommandExecute(ITreeNodeAsync<INetDiskFile> fileNode)
        {
            var tokens = await NetDiskUser.DownloadAsync(fileNode);

            GlobalMessageQueue.Enqueue(
                fileNode.Content.FileType == FileTypeEnum.FolderType
                    ? $"Added \"{fileNode.Content.FilePath.FileName}\" folder (includes {tokens.Count} files) to Download list."
                    : $"Added \"{fileNode.Content.FilePath.FileName}\" to Download list.");
            EventAggregator.GetEvent<DownloadTaskCreatedEvent>().Publish(tokens);
        }

        private async void UpdateChildrenCacheAsync()
        {
            await CurrentFolder.TryGetChildrenAsync();
            OnPropertyChanged(nameof(CurrentFolder));
        }

    }
}

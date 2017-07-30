using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Accelerider.Windows.Commands;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.ViewModels
{
    public class NetDiskFilesViewModel : ViewModelBase
    {
        public NetDiskFilesViewModel(IUnityContainer container) : base(container)
        {
            EnterFolderCommand = new RelayCommand<ITreeNodeAsync<INetDiskFile>>(
                file => CurrentFolder = file,
                file => file?.Content?.FileType == FileTypeEnum.FolderType
                );
            UpdateChildrenCacheCommand = new RelayCommand(UpdateChildrenCacheAsync);
        }

        protected override async Task LoadViewModel()
        {
            CurrentFolder = await NetDiskUser.GetNetDiskFileTreeAsync();
        }

        private ITreeNodeAsync<INetDiskFile> _currentFolder;
        public ITreeNodeAsync<INetDiskFile> CurrentFolder
        {
            get => _currentFolder;
            set
            {
                if (EqualityComparer<ITreeNodeAsync<INetDiskFile>>.Default.Equals(_currentFolder, value)) return;
                _currentFolder = value;
                UpdateChildrenCacheAsync();
            }
        }

        private ICommand _enterFolderCommand;
        public ICommand EnterFolderCommand
        {
            get => _enterFolderCommand;
            set => SetProperty(ref _enterFolderCommand, value);
        }


        private ICommand _updateChildrenCacheCommand;
        public ICommand UpdateChildrenCacheCommand
        {
            get => _updateChildrenCacheCommand;
            set => SetProperty(ref _updateChildrenCacheCommand, value);
        }

        private async void UpdateChildrenCacheAsync()
        {
            await CurrentFolder.TryGetChildrenAsync();
            OnPropertyChanged(nameof(CurrentFolder));
        }

    }
}

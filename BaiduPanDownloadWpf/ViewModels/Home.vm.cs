using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BaiduPanDownloadWpf.Commands;
using BaiduPanDownloadWpf.ViewModels.Items;
using Microsoft.Practices.Unity;
using BaiduPanDownloadWpf.Infrastructure.Interfaces;
using BaiduPanDownloadWpf.Infrastructure.Events;
using BaiduPanDownloadWpf.Infrastructure;
using BaiduPanDownloadWpf.Infrastructure.Interfaces.Files;

namespace BaiduPanDownloadWpf.ViewModels
{
    internal class HomeViewModel : ViewModelBase
    {
        private readonly ILocalDiskUserRepository _localDiskUserRepository;
        private ILocalDiskUser _localDiskUser;
        private NetDiskFileNodeViewModel _currentFile;
        private bool _isRefreshing;


        public HomeViewModel(IUnityContainer container, ILocalDiskUserRepository localDiskUserRepository)
            : base(container)
        {
            _localDiskUserRepository = localDiskUserRepository;

            // TODO: Replace the Command to Prism.Commands.DelegateCommand.
            ReturnFolderCommand = new Command(() => CurrentFile = CurrentFile.Parent, () => CurrentFile?.Parent != null);
            EnterFolderCommand = new Command<NetDiskFileNodeViewModel>(file => CurrentFile = file, file => file?.FileType == FileTypeEnum.FolderType);
            RefreshFileListCommand = new Command(RefreshFileListCommandExecuteAsync);
            BatchDownloadFileCommand = new Command<IList>(BatchDownloadFileCommandExecute, param => GetSeletedItems(param).Any());
            BatchDeleteFileCommand = new Command<IList>(BatchDeleteFileCommandExecute, param => GetSeletedItems(param).Any());

            EventAggregator.GetEvent<NetDiskUserSwitchedEvent>().Subscribe(OnNetDiskUserSwitched, Prism.Events.ThreadOption.UIThread);
        }

        public NetDiskFileNodeViewModel CurrentFile
        {
            get { return _currentFile; }
            set { SetProperty(ref _currentFile, value); }
        }
        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set { SetProperty(ref _isRefreshing, value); }
        }

        #region Commands and commands's logic
        private Command _returnFolderCommand;
        private Command<NetDiskFileNodeViewModel> _enterFolderCommand;
        private Command _refreshFileListCommand;
        private Command<IList> _batchDownloadFileCommand;
        private Command<IList> _batchDeleteFileCommand;

        public Command ReturnFolderCommand
        {
            get { return _returnFolderCommand; }
            set { SetProperty(ref _returnFolderCommand, value); }
        }
        public Command<NetDiskFileNodeViewModel> EnterFolderCommand
        {
            get { return _enterFolderCommand; }
            set { SetProperty(ref _enterFolderCommand, value); }
        }
        public Command RefreshFileListCommand
        {
            get { return _refreshFileListCommand; }
            set { SetProperty(ref _refreshFileListCommand, value); }
        }
        public Command<IList> BatchDownloadFileCommand
        {
            get { return _batchDownloadFileCommand; }
            set { SetProperty(ref _batchDownloadFileCommand, value); }
        }
        public Command<IList> BatchDeleteFileCommand
        {
            get { return _batchDeleteFileCommand; }
            set { SetProperty(ref _batchDeleteFileCommand, value); }
        }

        private async void RefreshFileListCommandExecuteAsync()
        {
            IsRefreshing = true;
            var refreshChildren = CurrentFile?.RefreshChildren();
            if (refreshChildren != null) await refreshChildren;
            IsRefreshing = false;
        }
        private void BatchDownloadFileCommandExecute(IList parameter)
        {
            foreach (var item in GetSeletedItems(parameter))
            {
                if (item.DownloadFileCommand.CanExecute())
                    item.DownloadFileCommand.Execute();
            }
        }
        private void BatchDeleteFileCommandExecute(IList parameter)
        {
            // Remove UI elements
            foreach (var item in GetSeletedItems(parameter))
            {
                if(item.DeleteFileCommand.CanExecute())
                    item.DeleteFileCommand.Execute();
            }
            // TODO: Invokes function from model.

        }

        private IEnumerable<NetDiskFileNodeViewModel> GetSeletedItems(IList parameter)
        {
            if (parameter == null) return new NetDiskFileNodeViewModel[0];

            var tempArray = new NetDiskFileNodeViewModel[parameter.Count];
            for (int i = 0; i < tempArray.Length; i++)
            {
                tempArray[i] = parameter[i] as NetDiskFileNodeViewModel;
            }
            return tempArray;
        }
        #endregion

        protected override void OnLoaded()
        {
            if (_localDiskUser?.CurrentNetDiskUser != null) return;

            _localDiskUser = _localDiskUserRepository.FirstOrDefault();
            if (_localDiskUser?.CurrentNetDiskUser != null)
            {
                CurrentFile = Container.Resolve<NetDiskFileNodeViewModel>(new DependencyOverride<INetDiskFile>(_localDiskUser.CurrentNetDiskUser.RootFile));
                CurrentFile?.RefreshChildren();
            }
        }

        public void OnNetDiskUserSwitched(NetDiskUserSwitchedEventArgs e)
        {
            //var currentUser = _localDiskUser.GetNetDiskUserById(e.UserId);
            //CurrentFile = Container.Resolve<NetDiskFileNodeViewModel>(new DependencyOverride<IDiskFile>(currentUser.RootFile));
        }
    }
}

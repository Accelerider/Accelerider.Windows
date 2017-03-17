using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BaiduPanDownloadWpf.Commands;
using BaiduPanDownloadWpf.ViewModels.Items;
using Microsoft.Practices.Unity;
using BaiduPanDownloadWpf.Infrastructure.Interfaces;
using BaiduPanDownloadWpf.Infrastructure;
using BaiduPanDownloadWpf.Infrastructure.Interfaces.Files;
using System.Threading.Tasks;

namespace BaiduPanDownloadWpf.ViewModels
{
    internal class HomeViewModel : ViewModelBase
    {
        private readonly IMountUserRepository _mountUserRepository;
        private INetDiskUser _netDiskUser;
        private NetDiskFileNodeViewModel _currentFile;
        private bool _isRefreshing;


        public HomeViewModel(IUnityContainer container, IMountUserRepository mountUserRepository)
            : base(container)
        {
            _mountUserRepository = mountUserRepository;

            // TODO: Replace the Command to Prism.Commands.DelegateCommand.
            ReturnFolderCommand = new Command(() =>
            {
                CurrentFile = CurrentFile.Parent;
                IsRefreshing = false;
            }, () => CurrentFile?.Parent != null);
            EnterFolderCommand = new Command<NetDiskFileNodeViewModel>(async file =>
            {
                CurrentFile = file;
                if (CurrentFile.Children == null) await RefreshFileListCommandExecuteAsync();
            }, file => file?.FileType == FileTypeEnum.FolderType);
            RefreshFileListCommand = new Command(async () => await RefreshFileListCommandExecuteAsync(), () => CurrentFile != null);
            BatchDownloadFileCommand = new Command<IList>(BatchDownloadFileCommandExecute, param => GetSeletedItems(param).Any());
            BatchDeleteFileCommand = new Command<IList>(BatchDeleteFileCommandExecute, param => GetSeletedItems(param).Any());
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

        private async Task RefreshFileListCommandExecuteAsync()
        {
            IsRefreshing = true;
            var refreshChildren = CurrentFile.RefreshChildren();
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
                if (item.DeleteFileCommand.CanExecute())
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
            if (SetProperty(ref _netDiskUser, _mountUserRepository?.FirstOrDefault()?.GetCurrentNetDiskUser()) && _netDiskUser != null)
            {
                CurrentFile = Container.Resolve<NetDiskFileNodeViewModel>(new DependencyOverride<INetDiskFile>(_netDiskUser.RootFile));
                if (RefreshFileListCommand.CanExecute()) RefreshFileListCommand.Execute();
            }
        }
    }
}

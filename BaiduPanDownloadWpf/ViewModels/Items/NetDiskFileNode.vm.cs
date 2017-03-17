using System.Collections.ObjectModel;
using BaiduPanDownloadWpf.Commands;
using BaiduPanDownloadWpf.Infrastructure;
using Microsoft.Practices.Unity;
using System.Threading.Tasks;
using BaiduPanDownloadWpf.Infrastructure.Interfaces.Files;
using BaiduPanDownloadWpf.Infrastructure.Interfaces;
using BaiduPanDownloadWpf.Infrastructure.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BaiduPanDownloadWpf.ViewModels.Items
{
    internal class NetDiskFileNodeViewModel : ViewModelBase
    {
        private readonly IMountUserRepository _mountUserRepository;
        private readonly INetDiskFile _netDiskFile;
        private NetDiskFileNodeViewModel _parent;
        private ObservableCollection<NetDiskFileNodeViewModel> _children;
        private bool _isDownloading;
        private int _downloadPercentage;

        public NetDiskFileNodeViewModel(IUnityContainer container, IMountUserRepository mountUserRepository, INetDiskFile netDiskFile)
            : base(container)
        {
            _mountUserRepository = mountUserRepository;
            _netDiskFile = netDiskFile;

            DeleteFileCommand = new Command(DeleteFileCommandExecuteAsync);
            DownloadFileCommand = new Command(DownloadFileCommandExecuteAsync);

            EventAggregator.GetEvent<DownloadStateChangedEvent>().Subscribe(
                OnDownloadStateChanged,
                Prism.Events.ThreadOption.UIThread,
                keepSubscriberReferenceAlive: false,
                filter: e => e.FileId == FileId);
            EventAggregator.GetEvent<DownloadProgressChangedEvent>().Subscribe(
                OnDownloadProgressChanged,
                Prism.Events.ThreadOption.UIThread,
                keepSubscriberReferenceAlive: false,
                filter: e => e.FileId == FileId);
        }

        public NetDiskFileNodeViewModel Parent
        {
            get { return _parent; }
            set { SetProperty(ref _parent, value); }
        }
        public ObservableCollection<NetDiskFileNodeViewModel> Children
        {
            get
            {
                //if (_children == null) RefreshChildren();
                return _children;
            }
            set { SetProperty(ref _children, value); }
        }
        public long FileId => _netDiskFile.FileId;
        public FileTypeEnum FileType => _netDiskFile.FileType;
        public DataSize? FileSize => _netDiskFile == null ? default(DataSize) : new DataSize(_netDiskFile.FileSize);
        public FileLocation FilePath => _netDiskFile.FilePath;
        public string MotifyTime => _netDiskFile?.MotifiedTime.ToString("yyyy-MM-dd HH:mm");
        public bool IsDownloading
        {
            get { return _isDownloading; }
            set { SetProperty(ref _isDownloading, value); }
        }
        public int DownloadPercentage
        {
            get { return _downloadPercentage; }
            set { SetProperty(ref _downloadPercentage, value); }
        }

        #region Commands and their logic
        private Command _deleteFileCommand;
        private Command _downloadFileCommand;

        public Command DeleteFileCommand
        {
            get { return _deleteFileCommand; }
            set { SetProperty(ref _deleteFileCommand, value); }
        }
        public Command DownloadFileCommand
        {
            get { return _downloadFileCommand; }
            set { SetProperty(ref _downloadFileCommand, value); }
        }

        private async void DeleteFileCommandExecuteAsync()
        {
            await _netDiskFile.DeleteAsync();
            await Parent.RefreshChildren(); // The result of the deletion is obtained by refreshing the list.
        }
        private async void DownloadFileCommandExecuteAsync()
        {
            if (FileType != FileTypeEnum.FolderType) IsDownloading = true;
            await _netDiskFile.DownloadAsync();
        }
        #endregion

        public Task RefreshChildren()
        {
            return Task.Run(async () =>
            {
                var children = new ObservableCollection<NetDiskFileNodeViewModel>();
                var downloadingFiles = _mountUserRepository.FirstOrDefault().GetCurrentNetDiskUser().GetUncompletedFiles();
                foreach (var item in await _netDiskFile.GetChildrenAsync())
                {
                    var child = Container.Resolve<NetDiskFileNodeViewModel>(new DependencyOverride<INetDiskFile>(item));
                    child.Parent = this;
                    child.IsDownloading = downloadingFiles?.Any(element => element.FileId == child.FileId) ?? false;
                    children.Add(child);
                }
                Children = children;
                OnPropertyChanged(nameof(FilePath));
            });
        }

        private void OnDownloadStateChanged(DownloadStateChangedEventArgs e)
        {
            if (e.NewState == DownloadStateEnum.Canceled
                || e.NewState == DownloadStateEnum.Completed
                || e.NewState == DownloadStateEnum.Faulted)
            {
                IsDownloading = false;
                DownloadPercentage = 0;
            }
        }
        private void OnDownloadProgressChanged(DownloadProgressChangedEventArgs e)
        {
            DownloadPercentage = (int)Math.Round((e.CurrentProgress / FileSize.Value) * 100);
            if (!IsDownloading) IsDownloading = true;
        }
    }
}

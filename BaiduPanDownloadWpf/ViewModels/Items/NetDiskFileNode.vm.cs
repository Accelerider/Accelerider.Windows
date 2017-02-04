using System.Collections.ObjectModel;
using BaiduPanDownloadWpf.Commands;
using BaiduPanDownloadWpf.Infrastructure;
using BaiduPanDownloadWpf.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;
using System.Threading.Tasks;

namespace BaiduPanDownloadWpf.ViewModels.Items
{
    internal class NetDiskFileNodeViewModel : ViewModelBase
    {
        private readonly INetDiskFile _netDiskFile;
        private NetDiskFileNodeViewModel _parent;
        private ObservableCollection<NetDiskFileNodeViewModel> _children;
        //private bool _isDataObsolete = true;

        public NetDiskFileNodeViewModel(IUnityContainer container, INetDiskFile netDiskFile)
            : base(container)
        {
            _netDiskFile = netDiskFile;

            DeleteFileCommand = new Command(DeleteFileCommandExecuteAsync);
            DownloadFileCommand = new Command(DownloadFileCommandExecuteAsync);
        }

        //public bool IsDataObsolete
        //{
        //    get { return _isDataObsolete ? _isDataObsolete : Parent?.IsDataObsolete ?? _isDataObsolete; } // The node is obsolete when its parent node is out of date
        //    set { _isDataObsolete = value; }
        //}
        public NetDiskFileNodeViewModel Parent
        {
            get { return _parent; }
            set { SetProperty(ref _parent, value); }
        }
        public ObservableCollection<NetDiskFileNodeViewModel> Children
        {
            get
            {
                if (_children == null) RefreshChildren();
                return _children;
            }
            set { SetProperty(ref _children, value); }
        }
        public long FileId => _netDiskFile.FileId;
        public FileTypeEnum FileType => _netDiskFile.FileType;
        public DataSize? FileSize => _netDiskFile == null ? default(DataSize) : new DataSize(_netDiskFile.FileSize);
        public FileLocation FilePath => _netDiskFile.FilePath;
        public string MotifyTime => _netDiskFile?.MotifyTime.ToString("yyyy-MM-dd HH:mm");

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
            await _netDiskFile.DownloadAsync();
        }
        #endregion

        public Task RefreshChildren()
        {
            //if (isDataObsolete) _isDataObsolete = true;
            return Task.Run(async () =>
            {
                //if (!IsDataObsolete) return;
                var children = new ObservableCollection<NetDiskFileNodeViewModel>();
                foreach (var item in await _netDiskFile.GetChildrenFileAsync())
                {
                    var child = Container.Resolve<NetDiskFileNodeViewModel>(new DependencyOverride<INetDiskFile>(item));
                    //child.IsDataObsolete = true; // Because current node has been refreshed, its children data are obsolete.
                    child.Parent = this;
                    children.Add(child);
                }
                Children = children;
                OnPropertyChanged(nameof(FilePath));
                //IsDataObsolete = false;
            });
        }
    }
}

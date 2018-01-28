using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using BaiduPanDownloadWpf.Commands;
using BaiduPanDownloadWpf.ViewModels.Items;
using BaiduPanDownloadWpf.Infrastructure;

namespace BaiduPanDownloadWpf.Controls
{
    /// <summary>
    /// Interaction logic for FolderLocationBar.xaml
    /// </summary>
    public partial class FolderLocationBar : INotifyPropertyChanged
    {
        public static readonly DependencyProperty CurrentFolderProperty = DependencyProperty.Register("CurrentFolder", typeof(NetDiskFileNodeViewModel), typeof(FolderLocationBar), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnCurrentFolderChanged));
        private static bool _isAddHistory = true;
        private static void OnCurrentFolderChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = sender as FolderLocationBar;
            if (control == null) return;

            var currentFolder = control.CurrentFolder;
            if (_isAddHistory)
            {
                if (currentFolder != null) control._historyArray.Add(currentFolder);
            }
            else
            {
                _isAddHistory = true;
            }
            var temp = new List<NetDiskFileNodeViewModel>();
            while (currentFolder.Parent != null)
            {
                temp.Add(currentFolder);
                currentFolder = currentFolder.Parent;
            }
            temp.Reverse();
            control.PART_LBFolderChain.ItemsSource = temp;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private NetDiskFileNodeViewModel _rootFolder;
        private readonly RecordableArray<NetDiskFileNodeViewModel> _historyArray = new RecordableArray<NetDiskFileNodeViewModel>(10);
        private ICommand _forwardCommand;
        private ICommand _backwardCommand;


        public NetDiskFileNodeViewModel CurrentFolder
        {
            get { return (NetDiskFileNodeViewModel)GetValue(CurrentFolderProperty); }
            set { SetValue(CurrentFolderProperty, value); }
        }
        public ICommand ForwardCommand
        {
            get { return _forwardCommand; }
            set { SetProperty(ref _forwardCommand, value); }
        }
        public ICommand BackwardCommand
        {
            get { return _backwardCommand; }
            set { SetProperty(ref _backwardCommand, value); }
        }


        public FolderLocationBar()
        {
            ForwardCommand = new Command(() =>
            {
                _isAddHistory = false;
                CurrentFolder = _historyArray.Forward();
            }, () => _historyArray.CanForward);
            BackwardCommand = new Command(() =>
            {
                _isAddHistory = false;
                CurrentFolder = _historyArray.Backward();
            }, () => _historyArray.CanBackward);
            InitializeComponent();
        }


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected virtual void SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return;
            storage = value;
            OnPropertyChanged(propertyName);
        }


        private void OnCurrentFolderSwitched(object sender, NetDiskFileNodeViewModel e)
        {
            CurrentFolder = e;
        }
        private void OnReturnRootFolderRequested(object sender, EventArgs e)
        {
            if (_rootFolder == null)
            {
                var temp = CurrentFolder;
                while (temp.Parent != null)
                {
                    temp = temp.Parent;
                }
                _rootFolder = temp;
            }
            CurrentFolder = _rootFolder;
        }
    }
}

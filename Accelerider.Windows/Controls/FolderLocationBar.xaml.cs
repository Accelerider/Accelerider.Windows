using Accelerider.Windows.Infrastructure.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Accelerider.Windows.Core;

namespace Accelerider.Windows.Controls
{
    /// <summary>
    /// Interaction logic for FolderLocationBar.xaml
    /// </summary>
    public partial class FolderLocationBar : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public static readonly DependencyProperty CurrentFolderProperty
            = DependencyProperty.Register("CurrentFolder", typeof(ITreeNodeAsync<NetDiskFile>), typeof(FolderLocationBar), new PropertyMetadata(null, OnCurrentFolderChanged));
        public ITreeNodeAsync<NetDiskFile> CurrentFolder
        {
            get => (ITreeNodeAsync<NetDiskFile>)GetValue(CurrentFolderProperty);
            set => SetValue(CurrentFolderProperty, value);
        }


        public FolderLocationBar()
        {
            InitializeComponent();
            CurrentFolder = MockData.MockData.GetNetDiskTreeNode();
        }

        private IList<ITreeNodeAsync<NetDiskFile>> _folderChain;
        public IList<ITreeNodeAsync<NetDiskFile>> FolderChain
        {
            get => _folderChain;
            set => SetProperty(ref _folderChain, value);
        }

        private static void OnCurrentFolderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (FolderLocationBar)d;
            var folderChain = new List<ITreeNodeAsync<NetDiskFile>>();
            var temp = control.CurrentFolder;
            do
            {
                folderChain.Add(temp);
            } while ((temp = temp.Parent) != null);
            folderChain.Reverse();
            control.FolderChain = folderChain;
        }


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private void ListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!((sender as ListBox)?.SelectedItem is ITreeNodeAsync<NetDiskFile> item)) return;
            CurrentFolder = item;
        }
    }
}

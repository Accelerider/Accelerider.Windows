using System;
using BaiduPanDownloadWpf.ViewModels.Items;
using System.Windows;
using System.Windows.Controls;

namespace BaiduPanDownloadWpf.Controls
{
    /// <summary>
    /// Interaction logic for FolderLocationItem.xaml
    /// </summary>
    public partial class FolderLocationItem
    {
        public static readonly DependencyProperty FolderProperty = DependencyProperty.Register("Folder", typeof(NetDiskFileNodeViewModel), typeof(FolderLocationItem), new PropertyMetadata(null));

        public NetDiskFileNodeViewModel Folder
        {
            get { return (NetDiskFileNodeViewModel)GetValue(FolderProperty); }
            set { SetValue(FolderProperty, value); }
        }

        public event EventHandler<NetDiskFileNodeViewModel> CurrentFolderChanged;

        public FolderLocationItem()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CurrentFolderChanged?.Invoke(this, Folder);
        }

        private void ListBox_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var listBox = sender as ListBox;

            var item = listBox?.SelectedItem as NetDiskFileNodeViewModel;
            if (item == null) return;

            PART_TgBtnShowSubFolders.IsChecked = false;
            CurrentFolderChanged?.Invoke(this, item);
        }
    }
}

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Accelerider.Windows.Modules.NetDisk.Models;

namespace Accelerider.Windows.Modules.NetDisk.Controls
{
    /// <summary>
    /// Interaction logic for NetDiskUserSelector.xaml
    /// </summary>
    public partial class NetDiskUserSelector
    {
        public static readonly DependencyProperty NetDiskUsersProperty = DependencyProperty.Register(
            "NetDiskUsers", typeof(ObservableCollection<INetDiskUser>), typeof(NetDiskUserSelector), new PropertyMetadata(default(ObservableCollection<INetDiskUser>)));

        public ObservableCollection<INetDiskUser> NetDiskUsers
        {
            get => (ObservableCollection<INetDiskUser>)GetValue(NetDiskUsersProperty);
            set => SetValue(NetDiskUsersProperty, value);
        }

        public static readonly DependencyProperty CurrentNetDiskUserProperty = DependencyProperty.Register(
            "CurrentNetDiskUser", typeof(INetDiskUser), typeof(NetDiskUserSelector), new PropertyMetadata(default(INetDiskUser)));

        public INetDiskUser CurrentNetDiskUser
        {
            get => (INetDiskUser)GetValue(CurrentNetDiskUserProperty);
            set => SetValue(CurrentNetDiskUserProperty, value);
        }

        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(
            "IsExpanded", typeof(bool), typeof(NetDiskUserSelector), new PropertyMetadata(default(bool)));

        public bool IsExpanded
        {
            get => (bool)GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        public double OpacityValue { get; } = 0.1;

        public NetDiskUserSelector()
        {
            InitializeComponent();
        }
    }
}

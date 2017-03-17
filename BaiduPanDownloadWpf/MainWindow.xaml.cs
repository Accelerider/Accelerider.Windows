using FirstFloor.ModernUI.Windows.Controls;
using System.ComponentModel;
using System.Windows;

namespace BaiduPanDownloadWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        public const string Key = "MainWindow";

        public MainWindow()
        {
            InitializeComponent();
            Application.Current.Resources[Key] = this;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            Application.Current.Resources.Remove(Key);
        }
    }
}

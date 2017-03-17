using FirstFloor.ModernUI.Windows.Controls;
using System.ComponentModel;
using System.Windows;

namespace BaiduPanDownloadWpf
{
    /// <summary>
    /// Interaction logic for SignInWindow.xaml
    /// </summary>
    public partial class SignWindow : ModernWindow
    {
        public const string Key = "SignInWindow";

        public SignWindow()
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

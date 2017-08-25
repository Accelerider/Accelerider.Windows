using System.Windows;
using System.Windows.Controls;

namespace Accelerider.Windows.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for AuthenticationBrowserWindow.xaml
    /// </summary>
    public partial class AuthenticationBrowserWindow
    {
        public static readonly DependencyProperty BrowserProperty = DependencyProperty.Register("Browser", typeof(WebBrowser), typeof(AuthenticationBrowserWindow), new PropertyMetadata(default(WebBrowser)));

        public WebBrowser Browser
        {
            get => (WebBrowser) GetValue(BrowserProperty);
            set => SetValue(BrowserProperty, value);
        }

        public AuthenticationBrowserWindow()
        {
            InitializeComponent();
        }
    }
}

using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BaiduPanDownloadWpf.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for LoginExceptionMessageDialog.xaml
    /// </summary>
    public partial class MessageDialog : ModernDialog
    {
        public MessageDialog(string title, string message)
        {
            InitializeComponent();
            TextBlock.Text = message;
            this.Title = title;

            // define the dialog buttons
            this.Buttons = new[] { this.OkButton };
        }
    }
}

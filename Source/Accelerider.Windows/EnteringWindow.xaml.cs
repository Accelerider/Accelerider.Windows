using System.ComponentModel;
using System.Windows;

namespace Accelerider.Windows
{
    /// <summary>
    /// Interaction logic for EnteringWindow.xaml
    /// </summary>
    public partial class EnteringWindow
    {
        public const string Key = "EnteringWindow";

        public EnteringWindow()
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

using System.ComponentModel;
using System.Windows;

namespace Accelerider.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public const string Key = "MainWindow";

        public MainWindow()
        {
            InitializeComponent();
            Application.Current.Resources[Key] = this;
            Loaded += SingletonProcess.OnWindowLoaded;
        }



        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            Application.Current.Resources.Remove(Key);
        }
    }
}

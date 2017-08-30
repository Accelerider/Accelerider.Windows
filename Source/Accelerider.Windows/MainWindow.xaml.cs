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
            WindowController.Register(this);
            Loaded += SingletonProcess.OnWindowLoaded;
        }
    }
}

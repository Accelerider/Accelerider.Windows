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
            WindowController.Register(this);
            Loaded += SingletonProcess.OnWindowLoaded;
        }
    }
}

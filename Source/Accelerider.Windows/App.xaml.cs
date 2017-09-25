using System.Windows;
using Accelerider.Windows.Common;

namespace Accelerider.Windows
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            new SingletonProcess().Check();

            base.OnStartup(e);

            new Bootstrapper().Run();
        }
    }
}

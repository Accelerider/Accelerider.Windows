using System.Windows;

namespace Accelerider.Windows
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ProcessController.CheckSingleton();

            base.OnStartup(e);

            new Bootstrapper().Run();
        }
    }
}

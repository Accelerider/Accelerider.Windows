using Accelerider.Windows.Properties;
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
            Settings.Default.PropertyChanged += (sender, eventArgs) => Settings.Default.Save();

            ProcessController.CheckSingleton();

            base.OnStartup(e);

            new Bootstrapper().Run();
        }
    }
}
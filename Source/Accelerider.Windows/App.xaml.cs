using System;
using System.Windows;
using MaterialDesignThemes.Wpf;
using Microsoft.Practices.Unity;

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

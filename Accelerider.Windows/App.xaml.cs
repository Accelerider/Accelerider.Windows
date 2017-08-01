using System;
using System.Windows;
using Accelerider.Windows.Core;
using MaterialDesignThemes.Wpf;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var container = new UnityContainer();
            ViewModels.ViewModelLocator.ViewModelFactory = type => container.Resolve(type);
            ConfigureContainer(container);
            container.Resolve<Module>().Initialize();
        }

        private void ConfigureContainer(IUnityContainer container)
        {
            container.RegisterInstance(typeof(SnackbarMessageQueue), new SnackbarMessageQueue(TimeSpan.FromSeconds(2)));
        }
    }
}

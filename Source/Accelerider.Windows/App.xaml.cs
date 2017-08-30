using System;
using System.Windows;
using Accelerider.Windows.Events;
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
            var container = new UnityContainer();
            ViewModels.ViewModelLocator.ViewModelFactory = type => container.Resolve(type);

            ConfigureContainer(container);
            container.Resolve<Core.Module>().Initialize();
            container.Resolve<Components.Authenticator.Module>().Initialize();
        }

        private void ConfigureContainer(IUnityContainer container)
        {
            container.RegisterInstance(typeof(SnackbarMessageQueue), new SnackbarMessageQueue(TimeSpan.FromSeconds(2)));
            container.RegisterInstance(typeof(EventAggregator), new EventAggregator());
        }
    }
}

using System;
using System.Windows;
using Accelerider.Windows.Events;
using Accelerider.Windows.Infrastructure.Interfaces;
using MaterialDesignThemes.Wpf;
using Microsoft.Practices.Unity;
using System.Net;

namespace Accelerider.Windows
{
    public class Bootstrapper
    {
        protected IUnityContainer Container { get; set; }


        public void Run()
        {
            Container = CreateContainer();
            ConfigureContainer();
            ConfigureViewModelLocator();
            InitializeModules();
            ConfigureApplicationEventHandlers();
            InitializeShell();
        }

        protected virtual IUnityContainer CreateContainer()
        {
            return new UnityContainer();
        }

        protected virtual void ConfigureContainer()
        {
            Container.RegisterInstance(new SnackbarMessageQueue(TimeSpan.FromSeconds(2)));
            Container.RegisterInstance(new EventAggregator());
        }

        protected virtual void ConfigureViewModelLocator()
        {
            ViewModels.ViewModelLocator.ViewModelFactory = type => Container.Resolve(type);
        }

        protected virtual void InitializeModules()
        {
            Container.Resolve<Core.Module>().Initialize();
            Container.Resolve<Components.Authenticator.Module>().Initialize();
        }

        protected virtual void InitializeShell()
        {
            ServicePointManager.DefaultConnectionLimit = 99999;
            WindowController.Show<EnteringWindow>();
        }

        protected virtual void ConfigureApplicationEventHandlers()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) => OnUnhandledException(sender, (Exception)e.ExceptionObject);
            Application.Current.DispatcherUnhandledException += (sender, e) => OnUnhandledException(sender, e.Exception);
            Application.Current.Exit += OnExit;
        }


        protected virtual void OnExit(object sender, ExitEventArgs e)
        {
            Container.Resolve<IAcceleriderUser>().OnExit();
        }

        protected virtual void OnUnhandledException(object sender, Exception exception)
        {

        }
    }
}

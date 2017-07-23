using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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
        }

        private void ConfigureContainer(IUnityContainer container)
        {
            
        }
    }
}

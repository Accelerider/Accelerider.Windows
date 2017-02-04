using BaiduPanDownloadWpf.Core;
using Prism.Unity;
using System.Windows;

namespace BaiduPanDownloadWpf
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return new MainWindow();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }

        protected override void InitializeModules()
        {
            Container.TryResolve<DownloadCoreModule>().Initialize();
        }
    }
}

using BaiduPanDownloadWpf.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;
using Prism.Unity;
using System.Windows;

namespace BaiduPanDownloadWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Bootstrapper _bootstrapper;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _bootstrapper = new Bootstrapper();
            _bootstrapper.Run();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            var localDiskUserRepository = (ILocalDiskUserRepository)_bootstrapper.Container.Resolve(typeof(ILocalDiskUserRepository));
            var localDiskUser = localDiskUserRepository?.FirstOrDefault();
            if (localDiskUser != null)
            {
                localDiskUserRepository.Save(localDiskUser);
            }
            base.OnExit(e);
        }
    }
}

using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.FileTransferService;
using Accelerider.Windows.Infrastructure.FileTransferService.Impls;
using Accelerider.Windows.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Logging;

namespace Accelerider.Windows.InfrastructureTests.TransportImpls
{
    [TestClass]
    public class ITransferServiceTests
    {
        [TestMethod]
        public void UseTest()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterInstance<ILoggerFacade>(new Logger());

            IConfigureFile configure = new ConfigureFile().Load(@"C:\Users\Dingp\Desktop\transfer-configure.json");
            configure.SetValue(TransferService.DownloaderContextKey, new TransferContextSettings { MaxParallelTranspoterCount = 4 });

            ITransferService service = new TransferService(container).Initialize(configure);

            service.Run();

            var downloader = service.Use<DefaultDownloaderBuilder>()
                .From("http://download.accelerider.com:888/纸上的魔法使.rar")
                .To(@"D:\Resources\Downloads\test-file.rar")
                .Configure(settings =>
                {
                    settings.MaxErrorCount = 3;
                    settings.AutoSwitchUri = true;
                    settings.ThreadCount = 1;
                })
                .Build();

            var managedToken = service.Register(downloader).AsManaged();

            managedToken.Ready();
            managedToken.Suspend();
            var configureFile = service.Shutdown();
        }

        private void OnStatusChanged(object sender, TransferStatusChangedEventArgs e)
        {

        }
    }
}

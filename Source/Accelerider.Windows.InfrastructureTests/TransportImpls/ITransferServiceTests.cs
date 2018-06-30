using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
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
            ServicePointManager.DefaultConnectionLimit = 99999;
            IUnityContainer container = new UnityContainer();
            container.RegisterInstance<ILoggerFacade>(new Logger());

            IConfigureFile configure = new ConfigureFile().Load(@"D:\transfer-configure.json");
            configure.SetValue(TransferService.DownloaderContextKey, new TransferContextSettings { MaxParallelTranspoterCount = 4 });

            ITransferService service = new TransferService(container).Initialize(configure);

            service.Run();

            var downloader = service.UseDefaultDownloaderBuilder()
                .From("http://download.accelerider.com:888/纸上的魔法使.rar")
                .To(@"D:\test-file.rar")
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
            managedToken.AsNext();

            var unmangedToken = service.Register(downloader).AsUnmanaged();

            unmangedToken.Start();
            unmangedToken.Suspend();
            unmangedToken.Dispose();


            var configureFile = service.Shutdown();


            Thread.Sleep(100000);
        }

        private void OnStatusChanged(object sender, TransferStatusChangedEventArgs e)
        {

        }
    }
}

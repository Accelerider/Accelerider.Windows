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
                .From("http://download.accelerider.com:888/%E7%BA%B8%E4%B8%8A%E7%9A%84%E9%AD%94%E6%B3%95%E4%BD%BF.rar")
                .To(@"F:\test-file.rar")
                .Configure(settings =>
                {
                    settings.MaxErrorCount = 3;
                    settings.AutoSwitchUri = true;
	                settings.BlockSize = DataSize.OneMB * 50;
                    settings.ThreadCount = 16;
                })
                .Build();

            var managedToken = service.Register(downloader).AsUnmanaged();
			managedToken.Start();
	        


            Thread.Sleep(100000);
        }

        private void OnStatusChanged(object sender, TransferStatusChangedEventArgs e)
        {

        }
    }
}

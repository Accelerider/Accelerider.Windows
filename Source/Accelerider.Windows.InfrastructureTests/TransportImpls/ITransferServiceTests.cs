using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.FileTransferService;
using Accelerider.Windows.Infrastructure.FileTransferService.Impls;
using Accelerider.Windows.Infrastructure.Interfaces;
using Autofac;
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

            var builder = new ContainerBuilder();
            builder.RegisterInstance(new Logger()).As<ILoggerFacade>();
            IContainer container = builder.Build();

            IConfigureFile configure = new ConfigureFile().Load(@"D:\transfer-configure.json");
            configure.SetValue(TransferService.DownloaderContextKey, new TransferContextSettings { MaxParallelTranspoterCount = 4 });

            ITransferService service = new TransferService(container).Initialize(configure);

            service.Run();

            var downloader = service.UseDefaultDownloaderBuilder()
                .From("http://download.accelerider.com:888/Made%20in%20Abyss.mkv")
                .To(@"D:\test-file2.rar")
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


            while (downloader.Status != TransferStatus.Completed)
            {
                Thread.Sleep(1000);
                Debug.WriteLine($"{downloader.Id}: {downloader.Status} - {downloader.CompletedSize}/{downloader.TotalSize} ");
            }
        }

        private void OnStatusChanged(object sender, TransferStatusChangedEventArgs e)
        {

        }
    }
}

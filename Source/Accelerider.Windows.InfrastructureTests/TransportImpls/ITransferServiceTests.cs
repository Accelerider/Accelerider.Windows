using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.FileTransferService;
using Accelerider.Windows.Infrastructure.FileTransferService.Impls;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accelerider.Windows.InfrastructureTests.TransportImpls
{
    [TestClass]
    public class ITransferServiceTests
    {
        [TestMethod]
        public void UseTest()
        {
            //ITransferService service = null;

            //var downloader = service.Use<ITransporterBuilder<IDownloader>>()
            //    .Configure(settings => { settings.AutoSwitchUri = true; })
            //    .From("http://download-link1/file1.rmvb")
            //    .From("http://download-link1/file2.rmvb")
            //    .From("http://download-link1/file3.rmvb")
            //    .From("http://download-link1/file4.rmvb")
            //    .To("D:/Users/Download/file.rmvb")
            //    .Build();

            //var token = downloader.Id;

            //service.Register(downloader).AsUnmanaged().Start();
            //service.Register(downloader).AsUnmanaged().Suspend();

            //service.Register(downloader).AsManaged().Suspend();
            //service.Register(downloader).AsManaged().AsNext();
            //service.Register(downloader).AsManaged().Ready();
            //service.Register(downloader).AsManaged().Cancel();

            var downloaderImpl = new DownloaderImpl();

            downloaderImpl.Update(
                new[] { new Uri("http://119.90.49.10/%E7%BA%B8%E4%B8%8A%E7%9A%84%E9%AD%94%E6%B3%95%E4%BD%BF.rar") },
                @"D:\Resources\Downloads\test.rar",
                new TransporterSettings
                {
                    //Headers = new h()
                });

            downloaderImpl.StatusChanged += OnStatusChanged;
            downloaderImpl.Start();

        }

        private void OnStatusChanged(object sender, TransferStatusChangedEventArgs e)
        {

        }
    }
}

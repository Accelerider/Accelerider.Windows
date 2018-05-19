using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.FileTransferService;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accelerider.Windows.InfrastructureTests.TransportImpls
{
    [TestClass]
    public class ITransferServiceTests
    {
        [TestMethod]
        public void UseTest()
        {
            ITransferService service = null;

            var downloader = service.Use<ITransporterBuilder<IDownloader>>()
                .Configure(settings => { settings.AutoSwitchUri = true; })
                .From("")
                .From("")
                .To("")
                .Build();

            var token = downloader.Token;

            service.Command(token).Restart();
            service.Command(token).AsNext();
            service.Command(token).Suspend();
            service.Command(token).Cancel();
        }

    }
}

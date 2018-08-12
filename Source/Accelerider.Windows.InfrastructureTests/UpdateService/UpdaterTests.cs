using Microsoft.VisualStudio.TestTools.UnitTesting;
using Accelerider.Windows.Infrastructure.UpdateService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.FileTransferService;
using Accelerider.Windows.Infrastructure.FileTransferService.Impls;
using Accelerider.Windows.Infrastructure.Interfaces;
using Autofac;
using Prism.Logging;

namespace Accelerider.Windows.Infrastructure.UpdateService.Tests
{
    [TestClass()]
    public class UpdaterTests
    {
        [TestMethod()]
        public void UpdaterTest()
        {

        }

        [TestMethod()]
        public async Task AddTest()
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(new Logger()).As<ILoggerFacade>();
            var container = builder.Build();

            IConfigureFile configure = new ConfigureFile().Load(@"D:\transfer-configure.json");
            configure.SetValue(TransferService.DownloaderContextKey, new TransferContextSettings { MaxParallelTranspoterCount = 4 });

            ITransferService service = new TransferService(container).Initialize(configure);

            service.Run();

        }

        [TestMethod()]
        public void AddTest1()
        {

        }

        [TestMethod()]
        public void RemoveTest()
        {

        }

        [TestMethod()]
        public void GetAvailableUpdatesAsyncTest()
        {

        }

        [TestMethod()]
        public void UpdateAsyncTest()
        {

        }
    }
}
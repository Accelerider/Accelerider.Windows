using Microsoft.VisualStudio.TestTools.UnitTesting;
using Accelerider.Windows.Infrastructure.TransportImpls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.FileTransferService;
using Accelerider.Windows.Infrastructure.FileTransferService.Impls;
using Accelerider.Windows.Infrastructure.Interfaces;
using Accelerider.Windows.InfrastructureTests;

namespace Accelerider.Windows.Infrastructure.TransportImpls.Tests
{
    [TestClass()]
    public class TransportSchedulerTests
    {
        private const int TaskCount = 1000;

        private readonly TransferContext<IDownloader> _downloadScheduler = new TransferContext<IDownloader>();
        private readonly TransferContext<IUploader> _uploadScheduler = new TransferContext<IUploader>();
        private readonly List<IDownloader> _tasks = GenerateTasks().ToList();

        private static IEnumerable<IDownloader> GenerateTasks()
        {
            for (int i = 0; i < TaskCount; i++)
            {
                yield return new MockDownloadTask();
            }
        }

        [TestMethod()]
        public void StartAsyncTest()
        {

        }

        [TestMethod()]
        public void RecordAsyncTestAsync()
        {
            Parallel.ForEach(_tasks, item => _downloadScheduler.Record(item));

            var tasks = _downloadScheduler.GetAllTasks();

            _downloadScheduler.Start();
        }

        [TestMethod()]
        public void GetAllTasksTest()
        {

        }

        [TestMethod()]
        public void ShutdownAsyncTest()
        {

        }
    }
}
using System.Collections.Generic;
using System.Linq;
using Accelerider.Windows.Infrastructure.FileTransferService;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accelerider.Windows.InfrastructureTests.TransportImpls
{
    [TestClass()]
    public class TransportSchedulerTests
    {
        private const int TaskCount = 1000;

        //private readonly TransferContext<IDownloader> _downloadScheduler = new TransferContext<IDownloader>();
        //private readonly TransferContext<IUploader> _uploadScheduler = new TransferContext<IUploader>();
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
            TransporterId token1 = new TransporterId();
            TransporterId token2 = new TransporterId();
          
            var r1 = Equals(token1, token2);
            var r2 = EqualityComparer<TransporterId>.Default.Equals(token1, token2);
            var r3 = token1.Equals(token2);
            var r4 = token1 == token2;
        }

        [TestMethod()]
        public void RecordAsyncTestAsync()
        {
            //Parallel.ForEach(_tasks, item => _downloadScheduler.Add(item));

            //var tasks = _downloadScheduler.GetAllTasks();

            //_downloadScheduler.Run();
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
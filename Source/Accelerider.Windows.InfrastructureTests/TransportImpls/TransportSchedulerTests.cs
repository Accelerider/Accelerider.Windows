using Microsoft.VisualStudio.TestTools.UnitTesting;
using Accelerider.Windows.Infrastructure.TransportImpls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Interfaces;
using Accelerider.Windows.InfrastructureTests;

namespace Accelerider.Windows.Infrastructure.TransportImpls.Tests
{
    [TestClass()]
    public class TransportSchedulerTests
    {
        private const int TaskCount = 1000;

        private readonly TransportScheduler<IDownloadTask> _downloadScheduler = new TransportScheduler<IDownloadTask>();
        private readonly TransportScheduler<IUploadTask> _uploadScheduler = new TransportScheduler<IUploadTask>();
        private readonly List<IDownloadTask> _tasks = GenerateTasks().ToList();

        private static IEnumerable<IDownloadTask> GenerateTasks()
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
        public async Task RecordAsyncTestAsync()
        {
            Parallel.ForEach(_tasks, async item => await _downloadScheduler.RecordAsync(item));

            var tasks = _downloadScheduler.GetAllTasks();

            await _downloadScheduler.StartAsync();
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
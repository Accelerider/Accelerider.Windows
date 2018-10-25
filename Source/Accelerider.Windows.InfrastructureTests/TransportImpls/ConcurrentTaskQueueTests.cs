using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.FileTransferService;
using Accelerider.Windows.Infrastructure.FileTransferService.Impls;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accelerider.Windows.InfrastructureTests.TransportImpls
{
    [TestClass()]
    public class ConcurrentTaskQueueTests
    {
        private const int TaskCount = 1000;

        private readonly ConcurrentTransporterQueue<ITransporter> _queue = new ConcurrentTransporterQueue<ITransporter>();
        private readonly List<ITransporter> _tasks = GenerateTasks().ToList();

        private static IEnumerable<ITransporter> GenerateTasks()
        {
            for (int i = 0; i < TaskCount; i++)
            {
                yield return new MockTransportTask();
            }
        }


        [TestMethod()]
        public void EnqueueTest()
        {
            Parallel.ForEach(_tasks, task => _queue.Enqueue(task));

            Assert.AreEqual(TaskCount, _queue.Count);
        }

        [TestMethod()]
        public void DequeueTest()
        {
            // Concurrent
            EnqueueTest();

            Parallel.ForEach(_tasks, task => _queue.Dequeue());

            Assert.AreEqual(0, _queue.Count);

            // Ordered
            _queue.Clear();

            foreach (var task in _tasks)
            {
                _queue.Enqueue(task);
            }

            Assert.AreEqual(TaskCount, _queue.Count);

            var hashCodes1 = _tasks.Select(item => item.GetHashCode()).ToList();

            for (int i = 0; i < TaskCount; i++)
            {
                Assert.AreEqual(hashCodes1[i],_queue.Dequeue().GetHashCode());
            }
        }

        [TestMethod()]
        public void TopTest()
        {
            EnqueueTest();

            for (int i = 0; i < TaskCount; i++)
            {
                var lastItem = _tasks[TaskCount - i - 1];
                _queue.Top(lastItem);

                Assert.AreEqual(TaskCount, _queue.Count);
                //Assert.AreEqual(lastItem.GetHashCode(), _queue.Peek().GetHashCode());
            }
        }

        [TestMethod()]
        public void RemoveTest()
        {
            EnqueueTest();

            Parallel.ForEach(_tasks, task => _queue.Remove(task));

            Assert.AreEqual(0, _queue.Count);
        }

        [TestMethod()]
        public void GetEnumeratorTest()
        {

        }
    }
}
using System.Collections.Concurrent;

namespace Accelerider.Windows.Core.DownloadEngine
{
    internal class DownloadQueue
    {
        private readonly ConcurrentQueue<TransferTaskBase> _downloadingQueue = new ConcurrentQueue<TransferTaskBase>();
        private readonly ConcurrentQueue<TransferTaskBase> _preDownloadQueue = new ConcurrentQueue<TransferTaskBase>();
        private readonly ConcurrentQueue<TransferTaskBase> _waitingQueue = new ConcurrentQueue<TransferTaskBase>();

        public void Enqueue(TransferTaskBase task)
        {
            _waitingQueue.Enqueue(task);
            Requeue();
        }

        public void Clear()
        {
            Clear(_waitingQueue);
            Clear(_preDownloadQueue);
            Clear(_downloadingQueue);
        }

        private void Clear(ConcurrentQueue<TransferTaskBase> queue)
        {
            foreach (var item in queue)
            {
                item.TryCancelAsync();
            }
        }

        private void Requeue()
        {

        }
    }
}

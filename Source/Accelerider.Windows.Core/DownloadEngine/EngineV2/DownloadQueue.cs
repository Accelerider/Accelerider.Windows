using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerider.Windows.Core.DownloadEngine.EngineV2
{
    internal class DownloadQueue
    {
        private readonly ConcurrentQueue<DownloadTask> _createdTaskQueue; // Contains Created status, can be moved to waiting and suspended queue.
        private readonly ConcurrentQueue<DownloadTask> _waitingTaskQueue; // Contains Waiting status, can be moved to running and suspended queue.
        private readonly ConcurrentQueue<DownloadTask> _runningTaskQueue; // Contains Transferring status, can be moved to suspended queue.
        private readonly ConcurrentQueue<DownloadTask> _suspendedTaskQueue; // Contains Paused and Faulted status, can be moved to waiting queue.

        private void Test()
        {
            
        }
    }
}

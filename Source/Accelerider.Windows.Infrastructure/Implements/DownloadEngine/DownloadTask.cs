using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Infrastructure.Implements.DownloadEngine
{
    public class DownloadTask : ITransportTask
    {
        public bool Equals(ITransportTask other)
        {
            if (!(other is DownloadTask)) return false;
            return other.LocalPath.Equals(LocalPath) && other.Status == Status && other.TotalSize == TotalSize;
        }


        public event StatusChangedEventHandler StatusChanged;
        public bool IsCanceled { get; }
        public TransportStatus Status { get; private set; } = TransportStatus.Ready;
        public DataSize CompletedSize { get; private set; }
        public DataSize TotalSize { get; private set; }
        public FileLocation LocalPath { get; private set; }

        public Task StartAsync()
        {
            throw new NotImplementedException();
        }

        public Task SuspendAsync()
        {
            throw new NotImplementedException();
        }

        public Task RestartAsync()
        {
            throw new NotImplementedException();
        }

        public Task CancelAsync()
        {
            throw new NotImplementedException();
        }
    }
}

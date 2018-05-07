using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.InfrastructureTests
{
    public class MockTransportTask : ITransportTask
    {
        public bool Equals(ITransportTask other)
        {
            return Equals(this, other);
        }

        public event StatusChangedEventHandler StatusChanged;
        public bool IsDisposed { get; }
        public TransportStatus Status { get; }
        public DataSize CompletedSize { get; }
        public DataSize TotalSize { get; }
        public FileLocation LocalPath { get; }
        public Task StartAsync()
        {
            throw new NotImplementedException();
        }

        public Task SuspendAsync()
        {
            throw new NotImplementedException();
        }

        public Task DisposeAsync()
        {
            throw new NotImplementedException();
        }
    }
}

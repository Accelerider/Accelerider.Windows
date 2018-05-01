using System;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.Interfaces
{
    public delegate void StatusChangedEventHandler(ITransportTask sender, TransportStatus status);

    public interface ITransportTask : IEquatable<ITransportTask>
    {
        event StatusChangedEventHandler StatusChanged;

        bool IsCanceled { get; }

        TransportStatus Status { get; }

        DataSize CompletedSize { get; }

        DataSize TotalSize { get; }

        FileLocation LocalPath { get; }


        Task StartAsync();

        Task SuspendAsync();

        Task RestartAsync();

        Task CancelAsync();
    }
}

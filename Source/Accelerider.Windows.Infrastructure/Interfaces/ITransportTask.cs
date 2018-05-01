using System;
using System.Collections.Generic;
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


        Task StartAsync();

        Task SuspendAsync();

        Task RestartAsync();

        Task CancelAsync();
    }

    public interface IDownloadTask : ITransportTask
    {
        IReadOnlyCollection<Uri> FromPaths { get; }

        FileLocation ToPath { get; }
    }

    public interface IUploadTask : ITransportTask
    {
        FileLocation FromPath { get; }

        IReadOnlyCollection<Uri> ToPaths { get; }
    }
}

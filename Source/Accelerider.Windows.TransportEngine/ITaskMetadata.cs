using System;

namespace Accelerider.Windows.TransportEngine
{
    public interface ITaskMetadata : IEquatable<ITaskMetadata>
    {
        Uri Uri { get; }

        string LocalPath { get; }

        TransportTaskStatus Status { get; }

        long CompletedSize { get; }

        long Size { get; }
    }
}

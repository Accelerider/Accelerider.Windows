using System;

namespace Accelerider.Windows.TransportEngine
{
    public interface ITaskMetadata : IEquatable<ITaskMetadata>
    {
        Uri FromPath { get; }

        string ToPath { get; }

        TransportTaskStatus Status { get; }

        long CompletedSize { get; }

        long Size { get; }
    }
}

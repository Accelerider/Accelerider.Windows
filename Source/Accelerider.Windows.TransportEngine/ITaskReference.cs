using System;

namespace Accelerider.Windows.TransportEngine
{
    public interface ITaskReference : IEquatable<ITaskReference>
    {
        string FromPath { get; }

        string ToPath { get; }

        TransportStatus Status { get; }

        long CompletedSize { get; }

        long Size { get; }
    }
}

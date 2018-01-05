using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerider.Windows.TransferEngine
{
    public interface ITaskMetadata : IEquatable<ITaskMetadata>
    {
        Uri Uri { get; }

        string LocalPath { get; }

        TransferTaskStatus Status { get; }

        long CompletedSize { get; }

        long Size { get; }
    }
}

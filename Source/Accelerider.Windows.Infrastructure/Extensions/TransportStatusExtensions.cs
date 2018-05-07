using System.Collections.Generic;
using System.Linq;

namespace Accelerider.Windows.Infrastructure.Extensions
{
    public static class TransportStatusExtensions
    {
        private static readonly Dictionary<TransportStatus, TransportStatus[]> StatusChangeMapping = new Dictionary<TransportStatus, TransportStatus[]>
        {
            { TransportStatus.Ready, new []{ TransportStatus.Transporting, TransportStatus.Suspended, TransportStatus.Faulted } },
            { TransportStatus.Transporting, new []{ TransportStatus.Suspended, TransportStatus.Completed, TransportStatus.Faulted } },
            { TransportStatus.Suspended, new []{ TransportStatus.Ready, TransportStatus.Faulted } },
            { TransportStatus.Faulted, new []{ TransportStatus.Ready } }
        };

        public static bool CanConvertedTo(this TransportStatus @this, TransportStatus other) =>
            @this != TransportStatus.Completed && // The end status cannot be converted.
            StatusChangeMapping[@this].Contains(other);
    }
}

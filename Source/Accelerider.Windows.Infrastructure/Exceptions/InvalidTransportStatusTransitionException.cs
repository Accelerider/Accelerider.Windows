using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.Exceptions
{
    public class InvalidTransportStatusTransitionException : Exception
    {
        public InvalidTransportStatusTransitionException(TransportStatus oldStatus, TransportStatus newStatus)
            : base($"Cannot transfer from {oldStatus} to {newStatus}.")
        {
        }
    }
}

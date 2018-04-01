using System.Collections.Generic;
using System.Linq;

namespace Accelerider.Windows.Infrastructure.Extensions
{
    public static class TransferTaskStatusEnumExtensions
    {
        private static readonly Dictionary<TransferTaskStatusEnum, TransferTaskStatusEnum[]> StatusChangeMapping = new Dictionary<TransferTaskStatusEnum, TransferTaskStatusEnum[]>
        {
            { TransferTaskStatusEnum.Created, new []{ TransferTaskStatusEnum.Waiting } },
            { TransferTaskStatusEnum.Waiting, new []{ TransferTaskStatusEnum.Transferring, TransferTaskStatusEnum.Paused, TransferTaskStatusEnum.Faulted } },
            { TransferTaskStatusEnum.Transferring, new []{ TransferTaskStatusEnum.Paused, TransferTaskStatusEnum.Completed, TransferTaskStatusEnum.Faulted } },
            { TransferTaskStatusEnum.Paused, new []{ TransferTaskStatusEnum.Waiting, } },
            { TransferTaskStatusEnum.Faulted, new []{ TransferTaskStatusEnum.Waiting } },
        };

        public static bool CanConvertedTo(this TransferTaskStatusEnum self, TransferTaskStatusEnum other) =>
            !self.IsEndStatus() && // The end status cannot be converted.
            (other == TransferTaskStatusEnum.Canceled || // Any status can be Canceled except the end status.
             StatusChangeMapping[self].Contains(other));

        public static bool IsEndStatus(this TransferTaskStatusEnum self) => self == TransferTaskStatusEnum.Completed ||
                                                                            self == TransferTaskStatusEnum.Canceled;
    }
}

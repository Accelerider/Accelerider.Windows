using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Infrastructure
{
    public static class TreeNodeAsyncExtensions
    {
        public static async Task<IEnumerable<ITreeNodeAsync<T>>> FlattenAsync<T>(this ITreeNodeAsync<T> self)
        {
            return await new Soil<T>(self).FlattenAsync();
        }
    }

    public static class TransferStateEnumExtensions
    {
        private static readonly Dictionary<TransferStateEnum, TransferStateEnum[]> StateChangeMap = new Dictionary<TransferStateEnum, TransferStateEnum[]>
        {
            { TransferStateEnum.Waiting, new[]{ TransferStateEnum.Transfering, TransferStateEnum.Paused, TransferStateEnum.Canceled, TransferStateEnum.Faulted } },
            { TransferStateEnum.Transfering, new []{ TransferStateEnum.Paused, TransferStateEnum.Checking, TransferStateEnum.Completed, TransferStateEnum.Canceled, TransferStateEnum.Faulted } },
            { TransferStateEnum.Paused, new []{TransferStateEnum.Canceled, TransferStateEnum.Waiting, } }
        };

        private const TransferStateEnum EndState = TransferStateEnum.Completed | TransferStateEnum.Canceled | TransferStateEnum.Faulted;

        public static bool CanChangeTo(this TransferStateEnum self, TransferStateEnum other)
        {
            return self != (self & EndState) && StateChangeMap[self].Contains(other);
        }
    }
}

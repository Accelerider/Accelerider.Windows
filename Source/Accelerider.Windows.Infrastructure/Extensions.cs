using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Infrastructure
{
    public static class ITreeNodeAsyncExtensions
    {
        public static async Task<IReadOnlyList<ITreeNodeAsync<T>>> GetChildrenAsync<T>(this ITreeNodeAsync<T> self)
        {
            return await self.TryGetChildrenAsync() ? self.ChildrenCache : null;
        }

        //public static async Task<IEnumerable<ITreeNodeAsync<T>>> FlattenAsync<T>(this ITreeNodeAsync<T> self)
        //{
        //    // 1. Recursively gets children

        //    // 2. Return Ienumerable object
        //}

        //private static IEnumerable<ITreeNodeAsync<T>> Flatten<T>(this ITreeNodeAsync<T> node)
        //{
        //    yield return node;
        //    foreach (var child in node.ChildrenCache)
        //        foreach (var item in child.Flatten())
        //            yield return item;
        //}
    }

    public static class TransferStateEnumExtensions
    {
        private static readonly Dictionary<TransferStateEnum, TransferStateEnum[]> StateChangeMap = new Dictionary<TransferStateEnum, TransferStateEnum[]>
        {
            { TransferStateEnum.Waiting, new[]{ TransferStateEnum.Transfering, TransferStateEnum.Paused, TransferStateEnum.Canceled, TransferStateEnum.Faulted } },
            { TransferStateEnum.Transfering, new []{ TransferStateEnum.Paused, TransferStateEnum.Completed, TransferStateEnum.Canceled, TransferStateEnum.Faulted } },
            { TransferStateEnum.Paused, new []{TransferStateEnum.Canceled, TransferStateEnum.Waiting, } }
        };

        private const TransferStateEnum EndState = TransferStateEnum.Completed | TransferStateEnum.Canceled | TransferStateEnum.Faulted;

        public static bool CanChangeTo(this TransferStateEnum self, TransferStateEnum other)
        {
            return self != (self & EndState) && StateChangeMap[self].Contains(other);
        }
    }
}

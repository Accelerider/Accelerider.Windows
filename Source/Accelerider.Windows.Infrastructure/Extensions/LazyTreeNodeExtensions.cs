using System.Collections.Generic;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Infrastructure.Extensions
{
    public static class LazyTreeNodeExtensions
    {
        //public static async Task ForEachAsync<T>(this ILazyTreeNode<T> self, Action<T> action)
        //{
        //    await new Soil<T>(self).ForEachAsync(action);
        //}

        public static async Task<IEnumerable<ILazyTreeNode<T>>> FlattenAsync<T>(this ILazyTreeNode<T> self)
        {
            return await new Soil<T>(self).FlattenAsync();
        }

        public static async Task<IReadOnlyList<ILazyTreeNode<T>>> GetChildrenAsync<T>(this ILazyTreeNode<T> self, bool force = false)
        {
            return await new Soil<T>(self).GetChildrenAsync(force);
        }

        public static int Count<T>(this ILazyTreeNode<T> self)
        {
            return new Soil<T>(self).Count();
        }
    }
}

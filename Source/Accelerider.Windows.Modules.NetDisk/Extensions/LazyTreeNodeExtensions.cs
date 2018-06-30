using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Modules.NetDisk.Extensions
{
    public static class LazyTreeNodeExtensions
    {
        public static async Task<IEnumerable<ILazyTreeNode<T>>> FlattenAsync<T>(this ILazyTreeNode<T> self) =>
            await new Soil<T>(self).FlattenAsync();

        public static async Task<IReadOnlyList<ILazyTreeNode<T>>> GetChildrenAsync<T>(this ILazyTreeNode<T> self, bool force = false) =>
            await new Soil<T>(self).GetChildrenAsync(force);

        public static int Count<T>(this ILazyTreeNode<T> self) =>
            new Soil<T>(self).Count();

        private class Soil<T>
        {
            private readonly ILazyTreeNode<T> _seed;
            private Action<T> _action;


            public Soil(ILazyTreeNode<T> seed) => _seed = seed;

            public async Task<IEnumerable<ILazyTreeNode<T>>> FlattenAsync()
            {
                _action = null;
                await FlourishAsync(_seed);
                return Flatten(_seed);
            }

            public async Task<IReadOnlyList<ILazyTreeNode<T>>> GetChildrenAsync(bool force = false)
            {
                if (force || _seed.ChildrenCache == null)
                    await _seed.RefreshAsync();

                return _seed.ChildrenCache;
            }

            public async Task ForEachAsync(Action<T> action)
            {
                _action = action;
                await FlourishAsync(_seed);
            }

            public int Count() => Flatten(_seed).Count();

            private async Task FlourishAsync(ILazyTreeNode<T> seed) // TODO: Tail recursion / CPS
            {
                _action?.Invoke(seed.Content);
                if (await seed.RefreshAsync() && seed.ChildrenCache != null)
                {
                    foreach (var item in seed.ChildrenCache)
                    {
                        await FlourishAsync(item);
                    }
                }
            }

            private IEnumerable<ILazyTreeNode<T>> Flatten(ILazyTreeNode<T> node)
            {
                yield return node;
                if (node.ChildrenCache == null) yield break;
                foreach (var child in node.ChildrenCache)
                    foreach (var item in Flatten(child))
                        yield return item;
            }
        }
    }
}

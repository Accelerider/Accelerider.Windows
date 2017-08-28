using System.Collections.Generic;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Interfaces;
using System.Linq;
using System;

namespace Accelerider.Windows.Infrastructure
{
    internal class Soil<T>
    {
        private readonly ILazyTreeNode<T> _seed;
        private Action<T> _action;


        public Soil(ILazyTreeNode<T> seed)
        {
            _seed = seed;
        }

        public async Task<IEnumerable<ILazyTreeNode<T>>> FlattenAsync()
        {
            _action = null;
            await FlourishAsync(_seed);
            return Flatten(_seed);
        }

        public async Task<IReadOnlyList<ILazyTreeNode<T>>> GetChildrenAsync(bool force = false)
        {
            if (force || _seed.ChildrenCache == null)
            {
                await _seed.RefreshChildrenCacheAsync();
            }
            return _seed.ChildrenCache;
        }

        internal async Task ForEachAsync(Action<T> action)
        {
            _action = action;
            await FlourishAsync(_seed);
        }

        public int Count()
        {
            return Flatten(_seed).Count();
        }


        private async Task FlourishAsync(ILazyTreeNode<T> seed) // TODO: Tail recursion / CPS
        {
            _action?.Invoke(seed.Content);
            if (await seed.RefreshChildrenCacheAsync() && seed.ChildrenCache != null)
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

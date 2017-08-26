using System.Collections.Generic;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Interfaces;
using System.Linq;

namespace Accelerider.Windows.Infrastructure
{
    internal class Soil<T>
    {
        private readonly ILazyTreeNode<T> _seed;

        public Soil(ILazyTreeNode<T> seed)
        {
            _seed = seed;
        }

        public async Task<IEnumerable<ILazyTreeNode<T>>> FlattenAsync()
        {
            await Flourish(_seed);
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

        public int Count()
        {
            return Flatten(_seed).Count();
        }


        private async Task Flourish(ILazyTreeNode<T> seed) // TODO: Tail recursion / CPS
        {
            if (seed.ChildrenCache != null || await seed.RefreshChildrenCacheAsync())
            {
                foreach (var item in seed.ChildrenCache)
                {
                    await Flourish(item);
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

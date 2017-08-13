using System.Collections.Generic;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Infrastructure
{
    internal class Soil<T>
    {
        private readonly ITreeNodeAsync<T> _seed;

        public Soil(ITreeNodeAsync<T> seed)
        {
            _seed = seed;
        }

        public async Task<IEnumerable<ITreeNodeAsync<T>>> FlattenAsync()
        {
            await Flourish(_seed);
            return Flatten(_seed);
        }

        private async Task Flourish(ITreeNodeAsync<T> seed)
        {
            if (seed.ChildrenCache != null || await seed.TryGetChildrenAsync())
            {
                foreach (var item in seed.ChildrenCache)
                {
                    await Flourish(item);
                }
            }
        }

        private IEnumerable<ITreeNodeAsync<T>> Flatten(ITreeNodeAsync<T> node)
        {
            yield return node;
            if (node.ChildrenCache == null) yield break;
            foreach (var child in node.ChildrenCache)
                foreach (var item in Flatten(child))
                    yield return item;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Core
{
    internal class LazyTreeNode<T> : ILazyTreeNode<T>
    {
        private Func<T, Task<IEnumerable<T>>> _childrenProvider;
        private LazyTreeNode<T> _parent;
        private List<LazyTreeNode<T>> _childrenCache;


        public LazyTreeNode(T content)
        {
            Content = content;
        }


        public T Content { get; }

        public ILazyTreeNode<T> Root
        {
            get
            {
                ILazyTreeNode<T> temp = this;
                while (temp.Parent != null) temp = temp.Parent;
                return temp;
            }
        }

        public ILazyTreeNode<T> Parent => _parent;

        public IReadOnlyList<ILazyTreeNode<T>> Parents
        {
            get
            {
                var stack = new Stack<ILazyTreeNode<T>>();
                ILazyTreeNode<T> temp = this;
                while ((temp = temp.Parent) != null) stack.Push(temp);
                return (from item in stack select item).ToList();
            }
        }

        public IReadOnlyList<ILazyTreeNode<T>> ChildrenCache => _childrenCache?.AsReadOnly(); // TODO: WeakReference, but make sure it can be used at least once.

        public Func<T, Task<IEnumerable<T>>> ChildrenProvider
        {
            get => _childrenProvider ?? _parent.ChildrenProvider;
            set => _childrenProvider = value;
        }


        public async Task<bool> RefreshChildrenCacheAsync()
        {
            var enumerable = await ChildrenProvider(Content);
            var array = enumerable as T[] ?? enumerable?.ToArray();
            if (!array?.Any() ?? true)
            {
                _childrenCache = null;
                return false;
            }

            var treeNodes = (from item in array
                             select new LazyTreeNode<T>(item) { _parent = this })
                             .ToList();

            if (!treeNodes.Any())
            {
                _childrenCache = null;
                return false;
            }
            _childrenCache = treeNodes;
            return true;
        }

        public async Task ForEachAsync(Action<T> action)
        {
            await FlourishAsync(this, action);
        }


        private async Task FlourishAsync(LazyTreeNode<T> seed, Action<T> action) // TODO: CPS
        {
            action?.Invoke(seed.Content);
            if (await seed.RefreshChildrenCacheAsync())
            {
                foreach (var item in seed._childrenCache)
                {
                    await FlourishAsync(item, action);
                }
            }
        }
    }
}

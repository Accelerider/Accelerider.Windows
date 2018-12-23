using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure
{
    public class LazyTreeNode<T> : ILazyTreeNode<T>
    {
        private bool _isRefreshing;
        private Func<T, Task<IEnumerable<T>>> _childrenProvider;

        public LazyTreeNode(T content) => Content = content;

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

        public virtual ILazyTreeNode<T> Parent { get; protected set; }

        public IReadOnlyList<ILazyTreeNode<T>> Ancestors
        {
            get
            {
                var stack = new Stack<ILazyTreeNode<T>>();
                ILazyTreeNode<T> temp = this;
                while ((temp = temp.Parent) != null) stack.Push(temp);
                return (from item in stack select item).ToList().AsReadOnly();
            }
        }

        public virtual IReadOnlyList<ILazyTreeNode<T>> ChildrenCache { get; protected set; }

        public virtual Func<T, Task<IEnumerable<T>>> ChildrenProvider
        {
            get => _childrenProvider ?? (_childrenProvider = ((LazyTreeNode<T>)Parent)?.ChildrenProvider);
            set => _childrenProvider = value;
        }

        public virtual async Task<bool> RefreshAsync()
        {
            if (_isRefreshing) return false;
            _isRefreshing = true;

            if (ChildrenProvider == null) return AbortRefresh();
            var enumerable = await ChildrenProvider(Content);
            if (enumerable == null) return AbortRefresh();

            var collection = enumerable.ToList();
            if (!collection.Any()) return AbortRefresh();

            var children = collection.Select(GenerateLazyTreeNode).ToList();
            children.ForEach(item => item.Parent = this);

            SetChildrenCache(children.AsReadOnly());

            _isRefreshing = false;
            return true;
        }

        public async Task ForEachAsync(Action<T> callback, CancellationToken cancellationToken)
        {
            await ForEachAsync(this, callback, cancellationToken);
        }

        protected virtual LazyTreeNode<T> GenerateLazyTreeNode(T content) => new LazyTreeNode<T>(content);

        protected virtual void SetChildrenCache(IReadOnlyList<ILazyTreeNode<T>> childrenCache) => ChildrenCache = childrenCache;

        private static async Task ForEachAsync(ILazyTreeNode<T> node, Action<T> callback, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) return;

            if (node == null) return;

            callback?.Invoke(node.Content);
            await node.RefreshAsync();

            if (node.ChildrenCache == null) return;
            foreach (var child in node.ChildrenCache)
            {
                if (cancellationToken.IsCancellationRequested) return;

                await ForEachAsync(child, callback, cancellationToken);
            }
        }

        private bool AbortRefresh()
        {
            ChildrenCache = null;
            _isRefreshing = false;
            return false;
        }

        public override string ToString() => Content.ToString();

        public virtual void Release()
        {
            ChildrenCache = null;
            _childrenProvider = null;
        }
    }
}

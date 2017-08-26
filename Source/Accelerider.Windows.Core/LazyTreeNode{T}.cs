using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Core
{
    public class LazyTreeNode<T> : ILazyTreeNode<T>
    {
        private Func<T, Task<IEnumerable<T>>> _childrenProvider;


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
        public ILazyTreeNode<T> Parent { get; protected set; }
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
        public IReadOnlyList<ILazyTreeNode<T>> ChildrenCache { get; protected set; } // TODO: WeakReference

        public Func<T, Task<IEnumerable<T>>> ChildrenProvider
        {
            get => _childrenProvider ?? (Parent as LazyTreeNode<T>)?.ChildrenProvider;
            set => _childrenProvider = value;
        }


        public async Task<bool> RefreshChildrenCacheAsync()
        {
            var temp = await ChildrenProvider(Content);
            var enumerable = temp as T[] ?? temp?.ToArray();
            if (!enumerable?.Any() ?? true)
            {
                ChildrenCache = null;
                return false;
            }

            var treeNodes = (from item in enumerable
                             select new LazyTreeNode<T>(item) { Parent = this }).ToArray();

            if (!treeNodes.Any())
            {
                ChildrenCache = null;
                return false;
            }
            ChildrenCache = treeNodes;
            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Core
{
    public class AsyncTreeNode<T> : IAsyncTreeNode<T>
    {
        private Func<T, Task<IEnumerable<T>>> _childrenProvider;


        public AsyncTreeNode(T content)
        {
            Content = content;
        }


        public T Content { get; }
        public IAsyncTreeNode<T> Root
        {
            get
            {
                IAsyncTreeNode<T> temp = this;
                while (temp.Parent != null) temp = temp.Parent;
                return temp;
            }
        }
        public IAsyncTreeNode<T> Parent { get; protected set; }
        public IReadOnlyList<IAsyncTreeNode<T>> Parents
        {
            get
            {
                var stack = new Stack<IAsyncTreeNode<T>>();
                IAsyncTreeNode<T> temp = this;
                while ((temp = temp.Parent) != null) stack.Push(temp);
                return (from item in stack select item).ToList();
            }
        }
        public IReadOnlyList<IAsyncTreeNode<T>> ChildrenCache { get; protected set; } // TODO: WeakReference

        public Func<T, Task<IEnumerable<T>>> ChildrenProvider
        {
            get => _childrenProvider ?? (Parent as AsyncTreeNode<T>)?.ChildrenProvider;
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
                             select new AsyncTreeNode<T>(item) { Parent = this }).ToArray();

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

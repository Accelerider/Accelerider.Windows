using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accelerider.Windows.Core.Files;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Core
{
    public class TreeNodeAsync<T> : ITreeNodeAsync<T>
    {
        private NetDiskFile netDiskFile;

        public TreeNodeAsync(T content)
        {
            Content = content;
        }

        public T Content { get; }
        public ITreeNodeAsync<T> Root
        {
            get
            {
                ITreeNodeAsync<T> temp = this;
                while (temp.Parent != null) temp = temp.Parent;
                return temp;
            }
        }
        public ITreeNodeAsync<T> Parent { get; protected set; }
        public IReadOnlyList<ITreeNodeAsync<T>> Parents
        {
            get
            {
                var stack = new Stack<ITreeNodeAsync<T>>();
                ITreeNodeAsync<T> temp = this;
                while ((temp = temp.Parent) != null) stack.Push(temp);
                return (from item in stack select item).ToList();
            }
        }
        public IReadOnlyList<ITreeNodeAsync<T>> ChildrenCache { get; protected set; }
        private Func<T, Task<IEnumerable<T>>> _childrenProvider;
        public Func<T, Task<IEnumerable<T>>> ChildrenProvider
        {
            get => _childrenProvider ?? (Parent as TreeNodeAsync<T>)?.ChildrenProvider;
            set => _childrenProvider = value;
        }

        public async Task<bool> TryGetChildrenAsync()
        {
            var temp = from item in await ChildrenProvider(Content)
                       select new TreeNodeAsync<T>(item) { Parent = this };
            var tempArray = temp.ToArray();
            if (!tempArray.Any()) return false;
            ChildrenCache = tempArray;
            return true;
        }
    }
}

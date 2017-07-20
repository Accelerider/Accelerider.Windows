using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Core
{
    public class TreeNodeAsync<T> : ITreeNodeAsync<T>
    {
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
                while (temp.Parent != null)
                {
                    temp = temp.Parent;
                }
                return temp;
            }
        }

        public ITreeNodeAsync<T> Parent { get; private set; }

        public IEnumerable<ITreeNodeAsync<T>> ChildrenCache { get; set; }

        public Task<IEnumerable<ITreeNodeAsync<T>>> FindAllAsync(Func<ITreeNodeAsync<T>, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<ITreeNodeAsync<T>> FirstOrDefaultAsync(Func<ITreeNodeAsync<T>, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ITreeNodeAsync<T>>> FlattenAsync()
        {
            throw new NotImplementedException();
        }

        public Func<T, Task<IEnumerable<T>>> ChildrenProvider { get; set; }

        public async Task<IEnumerable<ITreeNodeAsync<T>>> GetChildrenAsync()
        {
            var temp = await ChildrenProvider(Content);
            return from x in temp select new TreeNodeAsync<T>(x) { Parent = this };
        }
    }
}

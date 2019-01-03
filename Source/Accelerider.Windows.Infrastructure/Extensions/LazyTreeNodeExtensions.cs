using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure
{
    public static class LazyTreeNodeExtensions
    {
        public static ILazyTreeNode<TTo> Convert<TFrom, TTo>(this ILazyTreeNode<TFrom> @this, Func<TFrom, TTo> converter = null)
        {
            if (@this is ILazyTreeNodeConverter<TTo, TFrom> lazyTreeNodeConverter)
            {
                return lazyTreeNodeConverter.Source;
            }

            return new LazyTreeNodeConverter<TFrom, TTo>(@this, converter);
        }

        private interface ILazyTreeNodeConverter<out TFrom, out TTo> : ILazyTreeNode<TTo>
        {
            ILazyTreeNode<TFrom> Source { get; }
        }

        private class LazyTreeNodeConverter<TFrom, TTo> : ILazyTreeNodeConverter<TFrom, TTo>, INotifyPropertyChanged
        {
            private readonly Func<TFrom, TTo> _converter;

            public ILazyTreeNode<TFrom> Source { get; }

            public LazyTreeNodeConverter(ILazyTreeNode<TFrom> source, Func<TFrom, TTo> converter)
            {
                Source = source ?? throw new ArgumentNullException(nameof(source));
                _converter = converter ?? throw new ArgumentNullException(nameof(converter));

                if (source is INotifyPropertyChanged notifyPropertyChanged)
                {
                    notifyPropertyChanged.PropertyChanged += (sender, args) => PropertyChanged?.Invoke(this, args);
                }
            }

            public TTo Content => _converter(Source.Content);

            public ILazyTreeNode<TTo> Root => Source.Root != null
                ? new LazyTreeNodeConverter<TFrom, TTo>(Source.Root, _converter)
                : null;

            public ILazyTreeNode<TTo> Parent => Source.Parent != null
                ? new LazyTreeNodeConverter<TFrom, TTo>(Source.Parent, _converter)
                : null;

            public IReadOnlyList<ILazyTreeNode<TTo>> Ancestors => Source
                .Ancestors?
                .Select(item => new LazyTreeNodeConverter<TFrom, TTo>(item, _converter))
                .ToList()
                .AsReadOnly();

            public IReadOnlyList<ILazyTreeNode<TTo>> ChildrenCache => Source
                .ChildrenCache?
                .Select(item => new LazyTreeNodeConverter<TFrom, TTo>(item, _converter))
                .ToList()
                .AsReadOnly();

            public async Task ForEachAsync(Action<TTo> callback, CancellationToken cancellationToken)
            {
                await Source.ForEachAsync(item => callback?.Invoke(_converter(item)), cancellationToken);
            }

            public async Task<bool> RefreshAsync() => await Source.RefreshAsync();

            public void Release()
            {
                throw new NotImplementedException();
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }
    }
}

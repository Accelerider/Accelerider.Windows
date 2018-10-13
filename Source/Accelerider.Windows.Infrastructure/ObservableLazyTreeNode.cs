using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Accelerider.Windows.Infrastructure
{
    public class ObservableLazyTreeNode<T> : LazyTreeNode<T>, INotifyPropertyChanged
    {
        private IReadOnlyList<ILazyTreeNode<T>> _childrenCache;

        public override IReadOnlyList<ILazyTreeNode<T>> ChildrenCache
        {
            get => _childrenCache;
            protected set => SetProperty(ref _childrenCache, value);
        }

        public ObservableLazyTreeNode(T content) : base(content)
        {
        }

        protected override LazyTreeNode<T> GenerateLazyTreeNode(T content) => new ObservableLazyTreeNode<T>(content);

        #region Implements INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual bool SetProperty<TProperty>(ref TProperty storage, TProperty value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<TProperty>.Default.Equals(storage, value)) return false;

            storage = value;
            RaisePropertyChanged(propertyName);

            return true;
        }

        #endregion
    }
}

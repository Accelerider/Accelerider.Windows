using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;


namespace BaiduPanDownloadWpf.Infrastructure
{
    public class TreeNode<T>
    {
        private readonly Collection<TreeNode<T>> _children = new Collection<TreeNode<T>>();
        private readonly bool _isInheritChildrenProviderFromParent;
        private Func<T, IEnumerable<T>> _childrenProvider;


        public T Content { get; }
        public TreeNode<T> Root => FirstParent(parent => parent.Parent == null) ?? this;
        public TreeNode<T> Parent { get; private set; }
        public IEnumerable<TreeNode<T>> Parents
        {
            get
            {
                var stack = new Stack<TreeNode<T>>();
                var temp = this;
                while ((temp = temp.Parent) != null) stack.Push(temp);
                return from item in stack select item;
            }
        }
        public virtual IEnumerable<TreeNode<T>> Children => ChildrenProvider != null
                                                  ? from item in ChildrenProvider(Content)
                                                    select new TreeNode<T>(item) { Parent = this }
                                                  : _children;
        public virtual Func<T, IEnumerable<T>> ChildrenProvider
        {
            get
            {
                return _isInheritChildrenProviderFromParent && _childrenProvider == null
                     ? _childrenProvider = FirstParent(parent => parent._childrenProvider != null)?._childrenProvider
                     : _childrenProvider;
            }
            set { _childrenProvider = value; }
        }


        public TreeNode(T content, bool isInheritChildrenProviderFromParent = true)
        {
            Content = content;
            _isInheritChildrenProviderFromParent = isInheritChildrenProviderFromParent;
        }


        public virtual void Add(TreeNode<T> child)
        {
            if (ChildrenProvider != null) throw new InvalidOperationException("The children provider already exist, should direct access to Children or ChildrenAsync property. ");
            if (child == null) return;
            if (child.Parent != null) throw new InvalidOperationException($"This instance already belongs to other node, the existing parent node is {child.Parent}. ");

            child.Parent = this;
            _children.Add(child);
        }
        public void Add(T child)
        {
            if (child == null) return;

            TreeNode<T> node = new TreeNode<T>(child);
            Add(node);
        }
        public void AddRange(IEnumerable<TreeNode<T>> children)
        {
            if (children == null) return;

            foreach (var item in children)
            {
                Add(item);
            }
        }
        public void AddRange(IEnumerable<T> children)
        {
            if (children == null) return;

            foreach (var item in children)
            {
                Add(item);
            }
        }
        public virtual bool Remove(TreeNode<T> child)
        {
            if (ChildrenProvider != null) throw new InvalidOperationException("The children provider already exist, don't allow to remove elements. ");
            if (child.Parent != this) throw new InvalidOperationException($"This instance is not a child node of current node, the parent node of it is {child.Parent}. ");

            var result = false;
            if (result = _children.Remove(child)) child.Parent = null;
            return result;
        }
        public int Remove(IEnumerable<TreeNode<T>> children)
        {
            var temp = children.ToArray();
            var count = 0;
            foreach (var item in temp)
            {
                if (Remove(item)) count++;
            }
            return count;
        }
        public TreeNode<T> RemoveAll()
        {
            Remove(_children);
            return this;
        }
        public TreeNode<T> Drop()
        {
            Parent?.Remove(this);
            return this;
        }
        public TreeNode<T> Free()
        {
            Drop();
            RemoveAll();
            return this;
        }
        public IEnumerable<TreeNode<T>> Flatten()
        {
            yield return this;
            foreach (var child in Children)
                foreach (var subChild in child.Flatten())
                    yield return subChild;
        }
        public IEnumerable<TreeNode<T>> FindAll(Func<TreeNode<T>, bool> predicate)
        {
            return from item in Root.Flatten()
                   where predicate(item)
                   select item;
        }
        public TreeNode<T> FirstOrDefault(Func<TreeNode<T>, bool> predicate)
        {
            return Root.Flatten().FirstOrDefault(predicate);
        }

        protected TreeNode<T> FirstParent(Predicate<TreeNode<T>> condition = null)
        {
            var temp = Parent;
            while (temp != null && (!condition?.Invoke(temp) ?? false)) temp = temp.Parent;
            return temp;
        }
    }
}


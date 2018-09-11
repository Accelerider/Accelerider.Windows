using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Modules.NetDisk.Models
{
	// Decompiled with JetBrains decompiler
	// Assembly: Accelerider.Windows.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
	// Type: Accelerider.Windows.Core.LazyTreeNode`1
	// TODO: Please rewrite this file.
	internal class LazyTreeNode<T> : ILazyTreeNode<T>
	{
		public LazyTreeNode(T content) => Content = content;

		private LazyTreeNode<T> _parent;

		private List<LazyTreeNode<T>> _childrenCache;

		private Func<T, Task<IEnumerable<T>>> _childrenProvider;

		public async Task<bool> RefreshAsync()
		{
			IEnumerable<T> objs = await this.ChildrenProvider(this.Content);
			IEnumerable<T> enumerable = objs;
			objs = (IEnumerable<T>)null;
			T[] objArray1 = enumerable as T[];
			if (objArray1 == null)
			{
				IEnumerable<T> source = enumerable;
				objArray1 = source != null ? source.ToArray<T>() : (T[])null;
			}
			T[] array = objArray1;
			T[] objArray2 = array;
			if (objArray2 == null || !((IEnumerable<T>)objArray2).Any<T>())
			{
				this._childrenCache = (List<LazyTreeNode<T>>)null;
				return false;
			}
			List<LazyTreeNode<T>> treeNodes = ((IEnumerable<T>)array).Select<T, LazyTreeNode<T>>((Func<T, LazyTreeNode<T>>)(item => new LazyTreeNode<T>(item)
			{
				_parent = this
			})).ToList<LazyTreeNode<T>>();
			if (!treeNodes.Any<LazyTreeNode<T>>())
			{
				this._childrenCache = (List<LazyTreeNode<T>>)null;
				return false;
			}
			this._childrenCache = treeNodes;
			return true;
		}

		public T Content { get; }

		public ILazyTreeNode<T> Root
		{
			get
			{
				var result = (ILazyTreeNode<T>)this;
				while (result.Parent != null)
					result = result.Parent;
				return result;
			}
		}

		public ILazyTreeNode<T> Parent => _parent;
		
		public IReadOnlyList<ILazyTreeNode<T>> Parents
		{
			get
			{
				var source = new Stack<ILazyTreeNode<T>>();
				var lazyTreeNode = (ILazyTreeNode<T>)this;
				while ((lazyTreeNode = lazyTreeNode.Parent) != null)
					source.Push(lazyTreeNode);
				return source.ToList();
			}
		}

		public IReadOnlyList<ILazyTreeNode<T>> ChildrenCache => _childrenCache?.AsReadOnly();

		public Func<T, Task<IEnumerable<T>>> ChildrenProvider
		{
			get => _childrenProvider ?? _parent.ChildrenProvider;
			set => _childrenProvider = value;
		}

		public async Task ForEachAsync(Action<T> action)
		{
			await this.FlourishAsync(this, action);
		}

		public async Task<bool> RefreshChildrenCacheAsync()
		{
			IEnumerable<T> objs = await this.ChildrenProvider(this.Content);
			IEnumerable<T> enumerable = objs;
			objs = (IEnumerable<T>)null;
			T[] objArray1 = enumerable as T[];
			if (objArray1 == null)
			{
				IEnumerable<T> source = enumerable;
				objArray1 = source != null ? source.ToArray<T>() : (T[])null;
			}
			T[] array = objArray1;
			T[] objArray2 = array;
			if (objArray2 == null || !((IEnumerable<T>)objArray2).Any<T>())
			{
				this._childrenCache = (List<LazyTreeNode<T>>)null;
				return false;
			}
			List<LazyTreeNode<T>> treeNodes = ((IEnumerable<T>)array).Select<T, LazyTreeNode<T>>((Func<T, LazyTreeNode<T>>)(item => new LazyTreeNode<T>(item)
			{
				_parent = this
			})).ToList<LazyTreeNode<T>>();
			if (!treeNodes.Any<LazyTreeNode<T>>())
			{
				this._childrenCache = (List<LazyTreeNode<T>>)null;
				return false;
			}
			this._childrenCache = treeNodes;
			return true;
		}

		private async Task FlourishAsync(LazyTreeNode<T> seed, Action<T> action)
		{
			Action<T> action1 = action;
			if (action1 != null)
				action1(seed.Content);
			bool flag = await seed.RefreshChildrenCacheAsync();
			if (!flag)
				return;
			foreach (LazyTreeNode<T> lazyTreeNode in seed._childrenCache)
			{
				LazyTreeNode<T> item = lazyTreeNode;
				await this.FlourishAsync(item, action);
				item = (LazyTreeNode<T>)null;
			}
			List<LazyTreeNode<T>>.Enumerator enumerator = new List<LazyTreeNode<T>>.Enumerator();
		}
	}
}

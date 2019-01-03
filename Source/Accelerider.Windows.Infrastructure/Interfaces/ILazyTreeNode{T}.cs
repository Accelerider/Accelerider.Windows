using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure
{
    /// <summary>
    /// Represents a tree structure whose <see cref="ChildrenCache"/> will be fetched or refreshed 
    /// when the <see cref="IRefreshable.RefreshAsync"/> method is called.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="Content"/> in the tree node.</typeparam>
    public interface ILazyTreeNode<out T> : IRefreshable
    {
        /// <summary>
        /// Gets the content that stored by the node.
        /// </summary>
        T Content { get; }

        /// <summary>
        /// Gets root of the node
        /// </summary>
        ILazyTreeNode<T> Root { get; }

        /// <summary>
        /// Gets parent of the node.
        /// </summary>
        ILazyTreeNode<T> Parent { get; }

        /// <summary>
        /// Get ancestors of the node, the order of the sequence is from the root to the parent of the node.
        /// (<see cref="Root"/> --> <see cref="Parent"/>)
        /// </summary>
        IReadOnlyList<ILazyTreeNode<T>> Ancestors { get; }

        /// <summary>
        /// Gets the cache of the child's node for the node.
        /// </summary>
        IReadOnlyList<ILazyTreeNode<T>> ChildrenCache { get; }

        /// <summary>
        /// Applies action to the content of this node and its children.
        /// </summary>
        /// <param name="callback">A operation to the content of nodes.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Returns a <see cref="Task"/> to wait.</returns>
        Task ForEachAsync(Action<T> callback, CancellationToken cancellationToken);

        /// <summary>
        /// Clear the <see cref="ChildrenCache"/> and the related data.
        /// </summary>
        void Release();
    }
}

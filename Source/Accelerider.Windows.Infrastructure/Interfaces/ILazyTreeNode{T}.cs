using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.Interfaces
{
    /// <summary>
    /// Represents a tree structure whose <see cref="ChildrenCache"/> will be fetched or refreshed 
    /// when the <see cref="RefreshChildrenCacheAsync"/> method is called.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ILazyTreeNode<out T>
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
        /// Get all the parents of the node, the order of the sequence is from the root to the parent of the node.
        /// (<see cref="Root"/> --> <see cref="Parent"/>)
        /// </summary>
        IReadOnlyList<ILazyTreeNode<T>> Parents { get; }

        /// <summary>
        /// Gets the cache of the child's node for the node.
        /// </summary>
        IReadOnlyList<ILazyTreeNode<T>> ChildrenCache { get; }

        /// <summary>
        /// Try to refresh the children of the node, 
        /// if true is returned, the data in <see cref="ChildrenCache"/> is up-to-date.
        /// </summary>
        /// <returns>Returns a <see cref="bool"/> type indicating whether the data was successfully fetched.</returns>
        Task<bool> RefreshChildrenCacheAsync();
    }

}

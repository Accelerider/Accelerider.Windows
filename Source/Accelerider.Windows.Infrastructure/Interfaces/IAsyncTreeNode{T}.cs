using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.Interfaces
{
    /// <summary>
    /// Represents a tree structure whose child nodes will be acquired when the <see cref="GetChildrenAsync"/> method is called.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAsyncTreeNode<out T>
    {
        /// <summary>
        /// Gets content of the node.
        /// </summary>
        T Content { get; }

        /// <summary>
        /// Gets root of the node
        /// </summary>
        IAsyncTreeNode<T> Root { get; }

        /// <summary>
        /// Gets parent of the node.
        /// </summary>
        IAsyncTreeNode<T> Parent { get; }

        /// <summary>
        /// Get all the parents of the node, the order of the sequence is from the root to the parent of the node.
        /// </summary>
        IReadOnlyList<IAsyncTreeNode<T>> Parents { get; }

        /// <summary>
        /// Gets the cache of the child's node for the node.
        /// </summary>
        IReadOnlyList<IAsyncTreeNode<T>> ChildrenCache { get; }

        /// <summary>
        /// Try to get the children of the node, 
        /// if true is returned, the children data is taken in <see cref="ChildrenCache"/>.
        /// </summary>
        /// <returns>Returns a <see cref="bool"/> type indicating whether the data was successfully fetched.</returns>
        Task<bool> RefreshChildrenCacheAsync();
    }

}

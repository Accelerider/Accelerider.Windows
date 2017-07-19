using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.Interfaces
{
    /// <summary>
    /// Represents a tree structure whose child nodes will be acquired when the <see cref="GetChildrenAsync"/> method is called.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITreeNodeAsync<T>
    {
        /// <summary>
        /// Gets content of the node.
        /// </summary>
        T Content { get; }

        /// <summary>
        /// Gets root of the node
        /// </summary>
        ITreeNodeAsync<T> Root { get; }

        /// <summary>
        /// Gets parent of the node.
        /// </summary>
        ITreeNodeAsync<T> Parent { get; }

        /// <summary>
        /// Gets children of the node.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> that can be get children of the node.</returns>
        Task<IEnumerable<ITreeNodeAsync<T>>> GetChildrenAsync();

        /// <summary>
        /// Returns a one-dimensional structure containing the node and its descendant nodes.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> that contain the node and its descendant nodes.</returns>
        Task<IEnumerable<ITreeNodeAsync<T>>> FlattenAsync();

        /// <summary>
        /// Returns the first child node that conforms to the predicate.
        /// </summary>
        /// <param name="predicate">Search condition.</param>
        /// <returns>First child node that conforms to the predicate.</returns>
        Task<ITreeNodeAsync<T>> FirstOrDefaultAsync(Func<ITreeNodeAsync<T>, bool> predicate);

        /// <summary>
        /// Returns all child nodes that conforms to the predicate.
        /// </summary>
        /// <param name="predicate">Search condition.</param>
        /// <returns>All child nodes that conforms to the predicate.</returns>
        Task<IEnumerable<ITreeNodeAsync<T>>> FindAllAsync(Func<ITreeNodeAsync<T>, bool> predicate);
    }
}

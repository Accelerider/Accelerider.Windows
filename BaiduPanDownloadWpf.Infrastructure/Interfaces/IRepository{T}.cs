using System.Collections.Generic;

namespace BaiduPanDownloadWpf.Infrastructure.Interfaces
{
    /// <summary>
    /// A interface for repository pattern.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    public interface IRepository<in TKey, TEntity>
    {
        /// <summary>
        /// Returns the first or default element in the collection.
        /// </summary>
        /// <returns>The first or default element, will return null if there are nothing in the collection.</returns>
        TEntity FirstOrDefault();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TEntity FindById(TKey id);

        /// <summary>
        /// Gets all instances.
        /// </summary>
        /// <returns></returns>
        IEnumerable<TEntity> GetAll();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Contains(TKey id);

        /// <summary>
        /// Persistences the entity parameter.
        /// </summary>
        void Save();
    }
}

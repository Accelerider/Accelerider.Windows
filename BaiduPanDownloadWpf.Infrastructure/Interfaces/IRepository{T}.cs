namespace BaiduPanDownloadWpf.Infrastructure.Interfaces
{
    /// <summary>
    /// A interface for repository pattern.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    public interface IRepository<TEntity>
    {
        /// <summary>
        /// Returns the first or default element in the collection.
        /// </summary>
        /// <returns>The first or default element, will return null if there are nothing in the collection.</returns>
        TEntity FirstOrDefault();

        /// <summary>
        /// Persistences the entity parameter.
        /// </summary>
        /// <param name="entity">The entity that needs to be persisted.</param>
        void Save(TEntity entity);
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPanDownloadWpf.Infrastructure.Interfaces
{
    /// <summary>
    /// A interface represents the repository of <see cref="IMountUser"/>.
    /// </summary>
    public interface IMountUserRepository : IRepository<string, IMountUser>
    {
        /// <summary>
        /// Remove a mount account. 
        /// </summary>
        /// <param name="id"></param>
        void Remove(string id);

        /// <summary>
        /// Creates a mount account or return an existing instance, and try to sign in.
        /// if the force is true, try to sign up.
        /// </summary>
        /// <param name="username">the name of user.</param>
        /// <param name="password">the password of the user.</param>
        /// <param name="force">Specifies whether registration is required if the instance does not exist.</param>
        /// <returns>An instance that is connected to the server.</returns>
        Task<IMountUser> CreateAsync(string username, string password, bool force = false);
    }
}

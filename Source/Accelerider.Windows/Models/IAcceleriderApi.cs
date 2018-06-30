using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Refit;

namespace Accelerider.Windows.Models
{
    [Headers("User-Agent: Accelerider.Windows.Wpf: v1.0.0-pre",
             "Accept-Language: en-US",
             "Authorization: Bearer")]
    internal interface IAcceleriderApi
    {
        // Accelerider account functions ------------------------------------------------
        [Get("/users/current")]
        Task<AcceleriderUser> GetCurrentUserAsync();

        [Patch("/users/current")]
        Task UpdateUserInfoAsync([Body] UserUpdateInfoBody updateInfo);

        [Delete("/tokens/{id}")]
        Task SignOutAsync(long id);

        // App store functions ----------------------------------------------------------
        [Get("/apps/children")]
        Task<IList<ModuleMetadata>> GetAllAppInfoAsync();

        [Get("/apps/{id}")]
        Task<ModuleMetadata> GetAppInfoByIdAsync(string id);

        [Get("/apps/{id}/content")]
        Task<Stream> GetAppByIdAsync(string id);
    }
}

using System.Threading.Tasks;
using Refit;

namespace Accelerider.Windows.Models
{
    [Headers("User-Agent: Accelerider.Windows.Wpf: v1.0.0-pre",
             "Accept-Language: en-US")]
    internal interface INonAuthenticationApi
    {
        [Post("/users")]
        Task SignUpAsync([Body] SignUpInfoBody signUpInfo);

        [Post("/tokens")]
        Task<string> LoginAsync([Body] LoginInfoBody loginInfo);
    }
}

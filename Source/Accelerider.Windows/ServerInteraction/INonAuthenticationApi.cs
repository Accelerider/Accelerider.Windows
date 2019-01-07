using System.Collections.Generic;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Modularity;
using Accelerider.Windows.Infrastructure.Upgrade;
using Refit;

namespace Accelerider.Windows.ServerInteraction
{
    [Headers(
        "User-Agent: Accelerider.Windows.Wpf: v1.0.0-pre", 
        "Accept-Language: en-US")]
    public interface INonAuthenticationApi
    {
        [Get("/verification")]
        Task<SendVerificationCodeResponse> SendVerificationCodeAsync([Query] string email);

        [Post("/users")]
        Task<SignUpResponse> SignUpAsync([Body] SignUpArgs args);

        [Post("/tokens")]
        Task<LoginResponse> LoginAsync([Body] LoginArgs args);

        [Get("/apps")]
        Task<List<AppMetadata>> GetAppMetadataList();
    }
}

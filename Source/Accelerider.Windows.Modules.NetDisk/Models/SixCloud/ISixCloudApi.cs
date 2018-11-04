using System.Threading.Tasks;
using Accelerider.Windows.Modules.NetDisk.Models.Results;
using Newtonsoft.Json;
using Refit;

namespace Accelerider.Windows.Modules.NetDisk.Models.SixCloud
{
    [Headers("User-Agent: Accelerider.Windows.Wpf: v1.0.0-pre",
        "Authorization: Bearer")]
    public interface ISixCloudApi
    {
        [Post("/v1/user/info")]
        Task<ResultBase> GetUserInfoAsync();

        [Post("/v1/files/page")]
        Task<ResultBase> GetFilesByPathAsync([Body(BodySerializationMethod.Json)] GetFilesByPathArgs args);

        [Post("/v1/files/get")]
        Task<ResultBase> GetFileInfoByPathAsync([Body(BodySerializationMethod.Json)] PathArg path);

        [Post("/v1/files/remove")]
        Task<ResultBase> RemoveFileByPathAsync([Body(BodySerializationMethod.Json)] PathArg data);

        [Post("/v1/user/sendRegisterMessage")]
        Task<ResultBase> SendRegisterMessageAsync([Body(BodySerializationMethod.Json)] PhoneArg phone);

        [Post("/v1/user/register")]
        Task<ResultBase> RegisterAsync([Body(BodySerializationMethod.Json)] RegisterData data);

        [Post("/v1/user/login")]
        Task<SixCloudLoginResult> LoginAsync([Body(BodySerializationMethod.Json)] LoginArgs args);
    }

    public class LoginArgs
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }

    public class GetFilesByPathArgs
    {
        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("pageSize")]
        public int PageSize { get; set; }
    }

	public class PathArg
	{
		[JsonProperty("path")]
		public string Path { get; set; }
	}

	public class PhoneArg
	{
		[JsonProperty("phone")]
		public string PhoneNumber { get; set; }
	}
}

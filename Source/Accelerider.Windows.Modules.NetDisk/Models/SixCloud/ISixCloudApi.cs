using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Modules.NetDisk.Models.Results;
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
		Task<ResultBase> GetFilesByPathAsync([Body(BodySerializationMethod.Json)] string path,
			[Body(BodySerializationMethod.Json)] int pageSize = 999);

		[Post("/v1/files/get")]
		Task<ResultBase> GetFileInfoByPathAsync([Body(BodySerializationMethod.Json)] string path);

		[Post("/v1/files/remove")]
		Task<ResultBase> RemoveFileByPathAsync([Body(BodySerializationMethod.Json)] string path);

		[Post("/v1/user/sendRegisterMessage")]
		Task<ResultBase> SendRegisterMessageAsync([Body(BodySerializationMethod.Json)] string phone);

		[Post("/v1/user/register")]
		Task<ResultBase> RegisterAsync([Body(BodySerializationMethod.Json)] RegisterData data);

		[Post("/v1/user/login")]
		Task<SixCloudLoginResult> LoginAsync([Body(BodySerializationMethod.Json)]string value, [Body(BodySerializationMethod.Json)][AliasAs("password")]string passwordMd5);


	}
}

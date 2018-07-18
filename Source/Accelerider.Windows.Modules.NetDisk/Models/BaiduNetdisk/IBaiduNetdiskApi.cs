using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Extensions;
using Accelerider.Windows.Modules.NetDisk.Models.Results;
using Refit;

namespace Accelerider.Windows.Modules.NetDisk.Models.BaiduNetdisk
{
	[Headers("User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36",
			 "Referer: https://pan.baidu.com/disk/home")]
	public interface IBaiduNetdiskApi
	{
		[Get("/disk/home")]
		Task<string> GetHomePageAsync();

		[Get("/api/list?channel=chunlei&clienttype=0&web=1&num=100&order=name")]
		Task<BaiduListFileResult> GetFileListAsync(string bdsToken, [AliasAs("dir")] string path, int page);

		[Post("/api/filemanager?opera=delete&channel=chunlei&web=1&app_id=250528&clienttype=8")]
		Task<ResultBase> DeleteFileAsync(string bdsToken, string logId, [Body(BodySerializationMethod.UrlEncoded)]
			Dictionary<string, string> fileList);

	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Modules.NetDisk.Models.Results;
using Refit;

namespace Accelerider.Windows.Modules.NetDisk.Models.Onedrive
{
	[Headers("User-Agent: Accelerider.Windows.Wpf: v1.0.0-pre",
		     "Authorization: Bearer")]
	public interface IMicrosoftGraphApi
	{
		[Get("/v1.0/me/drive")]
		Task<MicrosoftUserInfoResult> GetUserInfoAsync();

		[Get("/v1.0/me/drive/root/children")]
		Task<OnedriveListFileResult> GetRootFilesAsync();

		[Get("/v1.0/me/drive/root:{path}:/children")]
		Task<OnedriveListFileResult> GetFilesAsync(string path);
	}
}

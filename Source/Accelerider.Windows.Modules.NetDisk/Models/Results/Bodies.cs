using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Modules.NetDisk.Models.BaiduNetdisk;
using Accelerider.Windows.Modules.NetDisk.Models.Onedrive;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Accelerider.Windows.Modules.NetDisk.Models.Results
{
	public class BaiduListFileResult : ResultBase
	{
		[JsonProperty("list")]
		public List<BaiduNetdiskFile> FileList { get; set; }
	}

	public class OnedriveListFileResult : ResultBase
	{
		[JsonProperty("value")]
		public List<OnedriveFile> FileList { get; set; }

		[JsonProperty("@odata.nextLink")]
		public string NextPage { get; set; }
	}

	public class MicrosoftUserInfoResult : ResultBase
	{
		[JsonProperty("owner")]
		public JToken Owner { get; set; }

		[JsonProperty("quota")]
		public JToken Quota { get; set; }
	}

	public class SixCloudLoginResult : ResultBase
	{
		[JsonProperty("token")]
		public string Token { get; set; }
	}


}

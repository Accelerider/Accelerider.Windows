using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.WpfInteractions;
using Accelerider.Windows.Modules.NetDisk.Enumerations;
using Accelerider.Windows.Modules.NetDisk.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Accelerider.Windows.Modules.NetDisk.Models.Onedrive
{
	[JsonObject(MemberSerialization.OptIn)]
	public class OnedriveFile : INetDiskFile
	{
		public FileType FileType => _folder != null ? FileType.FolderType : FileType.OtherType;

		public FileLocator Path => _pathInfo?.Value<string>("path").Replace("/drive/root:",string.Empty) ?? "/";

		[JsonProperty("size")]
		public DisplayDataSize Size { get; set; }

		[JsonProperty("lastModifiedDateTime")]
		public DateTime ModifiedTime { get; set; }

		[JsonProperty("file")]
		private JToken _file;

		[JsonProperty("folder")]
		private JToken _folder;

		[JsonProperty("parentReference")]
		private JToken _pathInfo;



		public OnedriveUser Owner { get; set; }

		public async Task<bool> DeleteAsync()
		{
			return (await Owner.Api.DeleteFileAsync(Path)).Error == null;
		}
	}
}

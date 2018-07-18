using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Accelerider.Windows.Modules.NetDisk.Models.Results
{
	public class ResultBase
	{
		[JsonProperty("errno")]
		public int ErrorCode { get; set; }

		[JsonProperty("error")]
		public JToken Error { get; set; }

		[JsonProperty("request_id")]
		public long RequestId { get; set; }
	}
}

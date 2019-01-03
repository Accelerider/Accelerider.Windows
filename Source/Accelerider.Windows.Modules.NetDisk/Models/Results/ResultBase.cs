using System;
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

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("result")]
        public JToken Result { get; set; }

        [JsonProperty("request_id")]
        public long RequestId { get; set; }
    }

    public class Result<T>
    {
        public bool Success { get; set; }

        public T Value { get; set; }

        public virtual void Parse()
        {

        }
    }
}

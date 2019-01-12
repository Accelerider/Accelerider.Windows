using System;
using Newtonsoft.Json;

namespace Accelerider.Windows.Infrastructure.Upgrade
{
    public class UpgradeInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("version")]
        public Version Version { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("updateNotes")]
        public string UpdateNotes { get; set; }
    }
}

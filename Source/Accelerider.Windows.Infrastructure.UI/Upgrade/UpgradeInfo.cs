using System;
using System.Collections.Generic;
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

        [JsonProperty("moduleType")]
        public string ModuleType { get; set; }

        [JsonProperty("DependsOn")]
        public IEnumerable<string> DependsOn { get; set; }
    }
}

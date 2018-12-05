using System;
using Newtonsoft.Json;

namespace Accelerider.Windows.Infrastructure.JsonObjects
{
    public class AppVersion
    {
        [JsonProperty("stableVersion")]
        public Version StableVersion { get; set; }

        [JsonProperty("experimentalVersion")]
        public Version ExperimentalVersion { get; set; }

        [JsonProperty("experimentalPercentage")]
        public double ExperimentalPercentage { get; set; }
    }
}

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Accelerider.Windows.Infrastructure.JsonObjects
{
    public class AppMetadata
    {
        [JsonProperty("id")]
        public string Id { get; private set; }

        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; private set; }

        [JsonProperty("targetPlatform")]
        public string TargetPlatform { get; private set; }

        [JsonProperty("authors")]
        public List<string> Authors { get; private set; }

        [JsonProperty("keywords")]
        public List<string> Keywords { get; private set; }

        [JsonProperty("description")]
        public string Description { get; private set; }

        [JsonProperty("rate")]
        public double Rate { get; private set; }

        [JsonProperty("downloadCount")]
        public long DownloadCount { get; private set; }

        [JsonProperty("moduleType")]
        public string ModuleType { get; private set; }

        [JsonProperty("latestVersion")]
        public AppVersion LatestVersion { get; private set; }
    }
}

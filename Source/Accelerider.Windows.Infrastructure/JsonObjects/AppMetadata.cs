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

        [JsonProperty("releaseTime")]
        public long ReleaseTime { get; private set; }

        [JsonProperty("version")]
        public AppVersion Version { get; private set; }

        [JsonProperty("privateFiles")]
        public List<AppFileMetadata> PrivateFiles { get; private set; }

        [JsonProperty("publicFiles")]
        public List<AppFileMetadata> PublicFiles { get; private set; }
    }
}

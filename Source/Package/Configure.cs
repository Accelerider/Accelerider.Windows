using System.Collections.Generic;
using Newtonsoft.Json;

namespace Package
{
    internal class Configure
    {
        [JsonProperty]
        public string SourcePath { get; private set; }

        [JsonProperty]
        public string TargetPath { get; private set; }

        [JsonProperty]
        public string PackageNameFormat { get; private set; }

        [JsonProperty]
        public List<string> ItemNames { get; private set; }
    }
}

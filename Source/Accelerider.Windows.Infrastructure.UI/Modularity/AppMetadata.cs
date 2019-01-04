using System;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Prism.Modularity;

namespace Accelerider.Windows.Infrastructure.Modularity
{
    [JsonObject(MemberSerialization.OptIn)]
    public class AppMetadata : IModuleInfo
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string ModuleName { get; set; }

        [JsonProperty("version")]
        public Version Version { get; set; }

        [JsonProperty("url")]
        public string Ref { get; set; }

        [JsonProperty("moduleType")]
        public string ModuleType { get; set; }

        [JsonProperty("dependsOn")]
        public Collection<string> DependsOn { get; set; }

        public InitializationMode InitializationMode { get; set; } = InitializationMode.OnDemand;

        public ModuleState State { get; set; } = ModuleState.NotStarted;
    }
}

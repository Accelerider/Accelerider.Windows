using System;
using System.Collections.Generic;

namespace Accelerider.Windows.Models
{
    public class ModuleMetadata
    {
        public string Id { get; set; }

        public string AliasName { get; set; }

        public string TargetPlatform { get; set; }

        public IList<string> Authors { get; set; }

        public IList<string> Keywords { get; set; }

        public string Description { get; set; }

        public double Rate { get; set; }

        public long DownloadCount { get; set; }

        public DateTime ReleaseTime { get; set; }

        public DateTime CreatedTime { get; set; }

        public string Version { get; set; }

        public Uri LogoUrl { get; set; }

        public string ModuleName { get; set; }

        public string Checksum { get; set; }

        public string ModuleType { get; set; }

        public IList<string> Dependencies { get; set; }
    }
}

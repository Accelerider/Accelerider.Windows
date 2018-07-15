using System;
using System.Collections.Generic;

namespace Accelerider.Windows.Infrastructure.UpdateService
{
    public enum UpdateCheckResult
    {
        Skip,
        CanUpdate,
        Conflict
    }

    public abstract class AppInfoBase
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Checksum { get; set; }
    }

    public class SpecifiedVersionAppInfo : AppInfoBase
    {
        public Version Version { get; set; }
    }

    public class AppInfo : AppInfoBase
    {
        public Version LatestVersion { get; set; }

        public IReadOnlyList<DependencyAppInfo> Dependencies { get; set; }
    }

    public class DependencyAppInfo : AppInfo
    {
        public Version MixVersion { get; set; }

        public Version MaxVersion { get; set; }
    }
}

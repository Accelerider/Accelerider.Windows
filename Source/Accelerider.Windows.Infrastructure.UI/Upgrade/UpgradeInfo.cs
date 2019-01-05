using System;
using System.Collections.Generic;

namespace Accelerider.Windows.Infrastructure.Upgrade
{
    public class UpgradeInfo
    {
        public string Name { get; set; }

        public Version Version { get; set; }

        public string Url { get; set; }

        public string ModuleType { get; set; }

        public IEnumerable<string> DependsOn { get; set; }
    }
}

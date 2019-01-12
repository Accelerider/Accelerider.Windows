using System;
using System.Collections.ObjectModel;
using Prism.Modularity;

namespace Accelerider.Windows.Infrastructure.Modularity
{
    public class AppInfo : IModuleInfo
    {
        public string ModuleName { get; set; }

        public string ModuleType { get; set; }

        public Collection<string> DependsOn { get; set; }

        public Version Version { get; set; }

        public string UpdateNotes { get; set; }

        public string Ref { get; set; }

        public InitializationMode InitializationMode { get; set; }

        public ModuleState State { get; set; }
    }
}

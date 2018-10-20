using System.Collections.Generic;
using Prism.Modularity;

namespace Accelerider.Windows.Infrastructure.Modularity
{
    public class RemoteModuleInfo : ModuleInfo
    {
        public List<RemoteRef> RemoteRefs { get; set; }
    }
}

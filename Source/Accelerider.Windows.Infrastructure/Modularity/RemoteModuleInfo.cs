using System.Collections.Generic;
using Accelerider.Windows.RemoteReference;
using Prism.Modularity;

namespace Accelerider.Windows.Infrastructure.Modularity
{
    public class RemoteModuleInfo : ModuleInfo
    {
        public List<RemoteRef> RemoteRefs { get; set; }
    }
}

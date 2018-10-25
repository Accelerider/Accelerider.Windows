﻿using Prism.Events;
using System.Collections.Generic;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Modules.NetDisk.Interfaces;

namespace Accelerider.Windows.Modules.NetDisk
{
    public class CurrentNetDiskUserChangedEvent : PubSubEvent<INetDiskUser> { }

    public class IsLoadingFilesChangedEvent : PubSubEvent<bool> { }

    public class SelectedSearchResultChangedEvent : PubSubEvent<ILazyTreeNode<INetDiskFile>> { }

    public class SearchResultsChangedEvent : PubSubEvent<IEnumerable<ILazyTreeNode<INetDiskFile>>> { }
}

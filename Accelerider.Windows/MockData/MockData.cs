using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Core;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.MockData
{
    public static class MockData
    {
        private static readonly IList<string> FileList = new List<string>
        {
            @"E:\DingpingZhang\Documents\VSTS",
            @"E:\DingpingZhang\Documents\VSTS",
            @"E:\DingpingZhang\Documents\VSTS",
            @"E:\DingpingZhang\Documents\VSTS",
            @"E:\DingpingZhang\Documents\VSTS",
        };

        public static ITreeNodeAsync<INetDiskFile> GetNetDiskTreeNode()
        {
            var content = new NetDiskFile
            {
                FilePath = new FileLocation("/Home")
            };
            var result = new TreeNodeAsync<NetDiskFile>(content)
            {
                ChildrenProvider = node =>
                {
                    return Task.Run(() => from x in FileList
                                          select new NetDiskFile
                                          {
                                              FilePath = new FileLocation(x)
                                          });
                }
            };
            return result;
        }
    }
}

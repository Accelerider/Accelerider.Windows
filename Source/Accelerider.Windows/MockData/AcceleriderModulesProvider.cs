using System;
using System.Collections.Generic;
using Accelerider.Windows.Models;

namespace Accelerider.Windows.MockData
{
    public static class AcceleriderModulesProvider
    {
        public static IEnumerable<ModuleMetadata> GetData()
        {
            var temp1 = new ModuleMetadata
            {
                Id = 1,
                Authors = "Mrs4s, LD50",
                Description = "Baidu net-disk download tool without speed limit.",
                DownloadCount = 666,
                LogoUrl = new Uri("/Views/AppStore/MockData/cc.jpg", UriKind.Relative),
                ModuleName = "Net Disk",
                ModuleType = "Accelerider.Windows.Modules.NetDisk.NetDiskModule, Accelerider.Windows.Modules.NetDisk, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
                Rate = 4.9,
                TargetPlatform = "Accelerider.Windows.Wpf",
                Dependencies = new List<string>
                {
                    "Accelerider.Windows.Resources, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null; ModuleVersionId=6a73c0de-1b39-4bef-9726-3d39a0f1d0d1",
                    "Accelerider.Windows.Infrastructure, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null; ModuleVersionId=c05bae3c-3c5c-4f88-8ca8-879107eefb65",
                    "Accelerider.Windows.TransferEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null; ModuleVersionId=ed951bb1-b22b-4323-924b-439662b1d024"
                }
            };

            var temp2 = new ModuleMetadata
            {
                Id = 1,
                Authors = "Mrs4s, LD50",
                Description = "Share and communicate in Accelerider.",
                DownloadCount = 666,
                LogoUrl = new Uri("/Views/AppStore/MockData/lulxiu.jpg", UriKind.Relative),
                ModuleName = "Group",
                ModuleType = "Accelerider.Windows.Modules.Group.GroupModule, Accelerider.Windows.Modules.Group, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
                Rate = 4.9,
                TargetPlatform = "Accelerider.Windows.Wpf",
                Dependencies = new List<string>
                {
                    "Accelerider.Windows.Resources, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null; ModuleVersionId=6a73c0de-1b39-4bef-9726-3d39a0f1d0d1",
                    "Accelerider.Windows.Infrastructure, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null; ModuleVersionId=c05bae3c-3c5c-4f88-8ca8-879107eefb65",
                    "Accelerider.Windows.TransferEngine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null; ModuleVersionId=ed951bb1-b22b-4323-924b-439662b1d024"
                }
            };

            return new[] { temp1, temp2 };
        }
    }
}

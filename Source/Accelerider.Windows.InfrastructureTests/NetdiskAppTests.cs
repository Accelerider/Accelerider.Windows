using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Modules.NetDisk.Enumerations;
using Accelerider.Windows.Modules.NetDisk.Models.BaiduNetdisk;
using Accelerider.Windows.Modules.NetDisk.Models.Onedrive;
using Accelerider.Windows.Modules.NetDisk.Models.SixCloud;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accelerider.Windows.InfrastructureTests.TransportImpls
{
    [TestClass]
    public class NetdiskAppTests
    {
        [TestMethod]
        public void BaiduNetdiskTest()
        {
            var user = new BaiduNetdiskUser(
                "cookie");
            user.RefreshUserInfoAsync().GetAwaiter().GetResult();
            var files = user.GetFileRootAsync().GetAwaiter().GetResult();
            var root = files.Root.GetChildrenAsync().GetAwaiter().GetResult();
            Console.WriteLine(user.UserId);
        }

        [TestMethod]
        public void OnedriveTest()
        {
            var user = new OnedriveUser(
                "token");
            user.RefreshUserInfoAsync().GetAwaiter().GetResult();
            var files = user.GetFileRootAsync().GetAwaiter().GetResult();
            var root = files.Root.GetChildrenAsync().GetAwaiter().GetResult();
            var sub = root.First(v => v.Content.Type == FileType.FolderType).GetChildrenAsync().GetAwaiter().GetResult();
            Console.WriteLine(user.Username);
        }

        [TestMethod]
        public async void SixCloudTest()
        {
            //var user = new SixCloudUser();

            //user.RefreshUserInfoAsync().GetAwaiter().GetResult();
            //var files = user.GetFileRootAsync().GetAwaiter().GetResult();
            //Console.WriteLine(user.Phone);
        }
    }
}

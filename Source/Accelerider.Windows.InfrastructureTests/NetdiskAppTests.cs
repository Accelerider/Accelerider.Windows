using System;
using System.Linq;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Modules.NetDisk.Enumerations;
using Accelerider.Windows.Modules.NetDisk.Models.BaiduCloud;
using Accelerider.Windows.Modules.NetDisk.Models.OneDrive;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accelerider.Windows.InfrastructureTests.TransportImpls
{
    [TestClass]
    public class NetdiskAppTests
    {
        [TestMethod]
        public void BaiduNetdiskTest()
        {
            var user = new BaiduCloudUser(
                "cookie");
            user.RefreshAsync().GetAwaiter().GetResult();
            var files = user.GetFileRootAsync().GetAwaiter().GetResult();
            var root = files.Root.GetChildrenAsync().GetAwaiter().GetResult();
            Console.WriteLine(user.UserId);
        }

        [TestMethod]
        public void OnedriveTest()
        {
            var user = new OneDriveUser(
                "token");
            user.RefreshAsync().GetAwaiter().GetResult();
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

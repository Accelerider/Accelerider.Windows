using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Modules.NetDisk.Enumerations;
using Accelerider.Windows.Modules.NetDisk.Extensions;
using Accelerider.Windows.Modules.NetDisk.Models.BaiduNetdisk;
using Accelerider.Windows.Modules.NetDisk.Models.Onedrive;
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
			var root=files.Root.GetChildrenAsync().GetAwaiter().GetResult();
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
			var sub=root.First(v => v.Content.FileType == FileType.FolderType).GetChildrenAsync().GetAwaiter().GetResult();
			Console.WriteLine(user.Username);
		}
	}
}

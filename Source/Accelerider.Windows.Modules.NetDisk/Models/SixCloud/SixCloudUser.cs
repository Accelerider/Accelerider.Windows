using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Modules.NetDisk.Enumerations;
using Accelerider.Windows.Modules.NetDisk.Interfaces;
using Accelerider.Windows.Modules.NetDisk.Models.Onedrive;
using Newtonsoft.Json;
using Refit;

namespace Accelerider.Windows.Modules.NetDisk.Models.SixCloud
{
	public class SixCloudUser : NetDiskUser
	{

		public string AccessToken { get; private set; }

		public ISixCloudApi Api { get; }

		#region Userinfo
		public long Uuid { get; private set; }
		public string Name { get; private set; }
		public string Email { get; private set; }
		public string Phone { get; private set; }
		public long TotalSpace { get; private set; }
		public long UsedSpace { get; private set; }
		#endregion

		public SixCloudUser(string token)
		{
			AccessToken = token;
			Api = RestService.For<ISixCloudApi>(
				new HttpClient(new AuthenticatedHttpClientHandler(() => AccessToken))
				{
					BaseAddress = new Uri("https://api.6pan.cn")
				}, new RefitSettings()
				{
					JsonSerializerSettings = new JsonSerializerSettings()
				}
			);
		}


		public override Task DownloadAsync(ILazyTreeNode<INetDiskFile> from, FileLocator to, Action<TransferItem> callback)
		{
			throw new NotImplementedException();
		}

		public override Task<ILazyTreeNode<INetDiskFile>> GetFileRootAsync()
		{
			var tree = new LazyTreeNode<INetDiskFile>(new SixCloudFile()
			{
				Owner = this
			})
			{
				ChildrenProvider = async parent =>
				{
					var result = new List<SixCloudFile>();
					var json = await Api.GetFilesByPathAsync(parent.Path);
					if (!json.Success) return result;
					result = json.Result["list"].Select(v => v.ToObject<SixCloudFile>()).ToList();
					result.ForEach(v => v.Owner = this);
					return result;
				}
			};
			return Task.FromResult((ILazyTreeNode<INetDiskFile>)tree);
		}

		public override Task<IList<T>> GetFilesAsync<T>(FileCategory category)
		{
			throw new NotSupportedException("SixCloud not supported this method.");
		}

		public override async Task RefreshUserInfoAsync()
		{
			var result = await Api.GetUserInfoAsync();
			if (!result.Success) return;
			Uuid = result.Result["uuid"].ToObject<long>();
			Name = result.Result["name"].ToObject<string>();
			Email = result.Result["email"].ToObject<string>();
			Phone = result.Result["phone"].ToObject<string>();
			TotalSpace = result.Result["spaceCapacity"].ToObject<long>();
			UsedSpace = result.Result["spaceUsed"].ToObject<long>();

		}

		public override Task UploadAsync(FileLocator from, INetDiskFile to, Action<TransferItem> callback)
		{
			throw new NotImplementedException();
		}
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;
using Accelerider.Windows.Modules.NetDisk.Enumerations;
using Accelerider.Windows.Modules.NetDisk.Interfaces;
using Accelerider.Windows.Modules.NetDisk.Models.Results;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Refit;

namespace Accelerider.Windows.Modules.NetDisk.Models.Onedrive
{
	public class OnedriveUser : NetDiskUser
	{

		public string RefeshToken { get; }
		public string AccessToken { get; private set; }
		public IMicrosoftGraphApi Api { get; }

		public OnedriveUser(string token)
		{
			RefeshToken = token;
			Api = RestService.For<IMicrosoftGraphApi>(
				new HttpClient(new AuthenticatedHttpClientHandler(() => AccessToken))
				{
					BaseAddress = new Uri("https://graph.microsoft.com")
				}, new RefitSettings() { JsonSerializerSettings = new JsonSerializerSettings() });
		}

		public override async Task RefreshUserInfoAsync()
		{
			await RefreshAccessToken();
			var info = await Api.GetUserInfoAsync();
			Username = info.Owner["user"].Value<string>("displayName");
			TotalCapacity = info.Quota.Value<long>("total");
			UsedCapacity = info.Quota.Value<long>("used");
		}

		public async Task RefreshAccessToken()
		{
			using (var client = new HttpClient())
			{
				var json = JObject.Parse(
					await client.GetStringAsync("https://api.accelerider.com/v2/graph/accessToken?refeshToken=" + RefeshToken));
				AccessToken = json.Value<string>("accessToken");
			}
		}

		public override Task<ILazyTreeNode<INetDiskFile>> GetFileRootAsync()
		{
			var tree = new LazyTreeNode<INetDiskFile>(new OnedriveFile()
			{
				Owner = this
			})
			{
				ChildrenProvider = async parent =>
				{
					var result = new List<OnedriveFile>();
					if (parent.Path == "/")
					{
						result.AddRange((await Api.GetRootFilesAsync()).FileList);
					}
					else
					{
						var tmp = await Api.GetFilesAsync(parent.Path);
						result.AddRange(tmp.FileList);
						while (!string.IsNullOrEmpty(tmp.NextPage))
						{
							using (var client = new HttpClient()
							{
								DefaultRequestHeaders =
								{
									Authorization = new AuthenticationHeaderValue("Bearer",AccessToken)
								}
							})
							{
								tmp = JsonConvert.DeserializeObject<OnedriveListFileResult>(await client.GetStringAsync(tmp.NextPage));
								result.AddRange(tmp.FileList);
							}
						}
					}

					result.ForEach(v => v.Owner = this);
					return result;
				}
			};
			return Task.FromResult((ILazyTreeNode<INetDiskFile>)tree);
		}

		public override Task<IList<T>> GetFilesAsync<T>(FileCategory category)
		{
			throw new NotSupportedException("Onedrive not supported this method.");
		}

		public override Task DownloadAsync(ILazyTreeNode<INetDiskFile> @from, FileLocator to, Action<TransferItem> callback)
		{
			throw new NotImplementedException();
		}

		public override Task UploadAsync(FileLocator @from, INetDiskFile to, Action<TransferItem> callback)
		{
			throw new NotImplementedException();
		}
	}

	internal class AuthenticatedHttpClientHandler : HttpClientHandler
	{
		private readonly Func<string> _token;
		public AuthenticatedHttpClientHandler(Func<string> tokenAction)
		{
			_token = tokenAction;
		}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			if (_token != null)
				request.Headers.Authorization = new AuthenticationHeaderValue(request.Headers.Authorization.Scheme, _token.Invoke());
			return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
		}
	}

}


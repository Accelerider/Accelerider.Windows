using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Modules.NetDisk.Models.Results;
using Accelerider.Windows.TransferService;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Refit;

namespace Accelerider.Windows.Modules.NetDisk.Models.OneDrive
{
    public class OneDriveUser : NetDiskUserBase
    {

        public string RefreshToken { get; }
        public string AccessToken { get; private set; }
        public IMicrosoftGraphApi Api { get; }

        public OneDriveUser(string token)
        {
            RefreshToken = token;
            Api = RestService.For<IMicrosoftGraphApi>(
                new HttpClient(new AuthenticatedHttpClientHandler(() => AccessToken))
                {
                    BaseAddress = new Uri("https://graph.microsoft.com")
                }, new RefitSettings() { JsonSerializerSettings = new JsonSerializerSettings() });
        }

        public override async Task<bool> RefreshAsync()
        {
            try
            {
                await RefreshAccessToken();
                var info = await Api.GetUserInfoAsync();
                Username = info.Owner["user"].Value<string>("displayName");
                Capacity = (info.Quota.Value<long>("used"), info.Quota.Value<long>("total"));

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task RefreshAccessToken()
        {
            using (var client = new HttpClient())
            {
                var json = JObject.Parse(
                    await client.GetStringAsync("https://api.accelerider.com/v2/graph/accessToken?refeshToken=" + RefreshToken));
                AccessToken = json.Value<string>("accessToken");
            }
        }

        public override Task<ILazyTreeNode<INetDiskFile>> GetFileRootAsync()
        {
            var tree = new LazyTreeNode<INetDiskFile>(new OneDriveFile()
            {
                Owner = this
            })
            {
                ChildrenProvider = async parent =>
                {
                    var result = new List<OneDriveFile>();
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
                                tmp = JsonConvert.DeserializeObject<OneDriveListFileResult>(await client.GetStringAsync(tmp.NextPage));
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

        public override Task<IReadOnlyList<IDeletedFile>> GetDeletedFilesAsync()
        {
            throw new NotImplementedException();
        }

        public override Task<bool> DeleteFileAsync(INetDiskFile file)
        {
            throw new NotImplementedException();
        }

        public override Task<bool> RestoreFileAsync(IDeletedFile file)
        {
            throw new NotImplementedException();
        }

        public override IDownloadingFile Download(INetDiskFile @from, FileLocator to)
        {
            throw new NotImplementedException();
        }

        public override IReadOnlyList<IDownloadingFile> GetDownloadingFiles()
        {
            throw new NotImplementedException();
        }

        public override IReadOnlyList<ILocalDiskFile> GetDownloadedFiles()
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


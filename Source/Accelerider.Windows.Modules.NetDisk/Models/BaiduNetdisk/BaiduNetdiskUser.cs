using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Extensions;
using Accelerider.Windows.Modules.NetDisk.Enumerations;
using Accelerider.Windows.Modules.NetDisk.Interfaces;
using Newtonsoft.Json;
using Refit;

namespace Accelerider.Windows.Modules.NetDisk.Models.BaiduNetdisk
{
    public class BaiduNetdiskUser : NetDiskUserBase
    {

        public IBaiduNetdiskApi Api { get; }

        public string Cookie { get; }

        public string Token { get; private set; }

        public long UserId { get; set; }

        public BaiduNetdiskUser(string cookie)
        {
            Api = RestService.For<IBaiduNetdiskApi>(new HttpClient(new HttpClientHandler()
            {
                UseCookies = true,
                CookieContainer = cookie.ToCookieContainer(".baidu.com")
            })
            { BaseAddress = new Uri("https://pan.baidu.com") },
                new RefitSettings() { JsonSerializerSettings = new JsonSerializerSettings() });
            Cookie = cookie;
        }

        public override async Task RefreshUserInfoAsync()
        {
            var homePage = await Api.GetHomePageAsync();
            Token = homePage.GetMatch("\"bdstoken\":\"", "\",");
            UserId = long.Parse(homePage.GetMatch("\"uk\":", ",\"t"));
            Username = Regex.Unescape(homePage.GetMatch("\"username\":\"", "\","));

        }

        public override Task<ILazyTreeNode<INetDiskFile>> GetFileRootAsync()
        {
            var tree = new LazyTreeNode<INetDiskFile>(new BaiduNetdiskFile()
            {
                Owner = this,
                Path = "/"
            })
            {
                ChildrenProvider = async parent =>
                {
                    var result = new List<BaiduNetdiskFile>();
                    var page = 1;
                    do
                    {
                        var files = await Api.GetFileListAsync(Token, parent.Path, page);
                        if (files.FileList.Count == 0)
                            break;
                        result.AddRange(files.FileList);
                        page++;
                    } while (true);
                    result.ForEach(v => v.Owner = this);
                    return result;
                }
            };
            return Task.FromResult((ILazyTreeNode<INetDiskFile>)tree);
        }

        public override Task<IList<T>> GetFilesAsync<T>(FileCategory category)
        {
            throw new NotImplementedException();
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
}

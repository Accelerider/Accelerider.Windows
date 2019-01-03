using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Newtonsoft.Json;
using Refit;

namespace Accelerider.Windows.Modules.NetDisk.Models.BaiduCloud
{
    public class BaiduCloudUser : NetDiskUserBase
    {
        public IBaiduCloudApi Api { get; }

        public string Cookie { get; }

        public string Token { get; private set; }

        public long UserId { get; set; }

        public BaiduCloudUser(string cookie)
        {
            Api = RestService.For<IBaiduCloudApi>(
                new HttpClient(new HttpClientHandler
                {
                    UseCookies = true,
                    CookieContainer = cookie.ToCookieContainer(".baidu.com")
                })
                {
                    BaseAddress = new Uri("https://pan.baidu.com")
                },
                new RefitSettings
                {
                    JsonSerializerSettings = new JsonSerializerSettings()
                });

            Cookie = cookie;
        }

        public override async Task<bool> RefreshAsync()
        {
            try
            {
                var homePage = await Api.GetHomePageAsync();
                Token = homePage.GetMatch("\"bdstoken\":\"", "\",");
                UserId = long.Parse(homePage.GetMatch("\"uk\":", ",\"t"));
                Username = Regex.Unescape(homePage.GetMatch("\"username\":\"", "\","));

                return true;
            }
            catch
            {
                return false;
            }
        }

        public override Task<ILazyTreeNode<INetDiskFile>> GetFileRootAsync()
        {
            var tree = new LazyTreeNode<INetDiskFile>(new BaiduCloudFile())
            {
                ChildrenProvider = async parent =>
                {
                    var result = new List<BaiduCloudFile>();
                    var page = 1;
                    do
                    {
                        var files = await Api.GetFileListAsync(Token, parent.Path, page);
                        if (files.FileList.Count == 0)
                            break;
                        result.AddRange(files.FileList);
                        page++;
                    } while (true);
                    //result.ForEach(v => v.Owner = this);
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
}

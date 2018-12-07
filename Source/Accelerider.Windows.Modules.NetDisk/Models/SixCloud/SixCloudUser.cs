using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Modules.NetDisk.Enumerations;
using Accelerider.Windows.Modules.NetDisk.Interfaces;
using Accelerider.Windows.Modules.NetDisk.Models.OneDrive;
using Accelerider.Windows.TransferService;
using Newtonsoft.Json;
using Refit;
using Unity;

namespace Accelerider.Windows.Modules.NetDisk.Models.SixCloud
{
    public class SixCloudUser : NetDiskUserBase
    {
        [JsonProperty]
        public string AccessToken { get; private set; }

        public ISixCloudApi Api { get; }

        #region Userinfo

        public long Uuid { get; private set; }

        public string Email { get; private set; }

        public string Phone { get; private set; }

        public long TotalSpace { get; private set; }

        public long UsedSpace { get; private set; }

        #endregion

        protected IUnityContainer Container { get; }

        public SixCloudUser(IUnityContainer container)
        {
            Container = container;
            Avatar = new Uri("pack://application:,,,/Accelerider.Windows.Modules.NetDisk;component/Images/logo-six-cloud.png");
            Api = RestService.For<ISixCloudApi>(
                new HttpClient(new AuthenticatedHttpClientHandler(() => AccessToken)) { BaseAddress = new Uri("https://api.6pan.cn") },
                new RefitSettings { JsonSerializerSettings = new JsonSerializerSettings() }
            );
        }


        public override TransferItem Download(ILazyTreeNode<INetDiskFile> from, FileLocator to)
        {
            return MockDownload(from?.Content, to);
        }

        public override Task<ILazyTreeNode<INetDiskFile>> GetFileRootAsync()
        {
            var tree = new LazyTreeNode<INetDiskFile>(new SixCloudFile { Owner = this })
            {
                ChildrenProvider = async parent =>
                {
                    var result = new List<SixCloudFile>();
                    var json = await Api.GetFilesByPathAsync(new GetFilesByPathArgs { Path = parent.Path, PageSize = 999 });
                    if (!json.Success) return result;
                    result = json.Result["list"].Select(v => v.ToObject<SixCloudFile>()).ToList();
                    result.ForEach(v => v.Owner = this);
                    return result;
                }
            };
            return Task.FromResult((ILazyTreeNode<INetDiskFile>)tree);
        }

        protected override IDownloaderBuilder ConfigureDownloaderBuilder(IDownloaderBuilder builder)
        {
            return builder.UseSixCloudConfigure();
        }

        protected override IRemotePathProvider GetRemotePathProvider(string jsonText)
        {
            return new SixCloudRemotePathProvider(jsonText, this);
        }

        public async Task LoginAsync(string value, string password)
        {
            if (!string.IsNullOrWhiteSpace(value) &&
                !string.IsNullOrWhiteSpace(password))
            {
                var result = await Api.LoginAsync(new LoginArgs
                {
                    Value = value,
                    Password = password.ToMd5()
                });
                if (result.Success)
                {
                    AccessToken = result.Token;
                }
            }
        }

        public async Task<string> SendSmsAsync(string phoneNumber)
        {
            var result = await Api.SendRegisterMessageAsync(new PhoneArgs() { PhoneNumber = phoneNumber });
            return result.Success ? result.Result.ToObject<string>() : null;
        }

        public async Task SignUpAsync(string username, string password, string passCode, string phoneInfo)
        {
            if (!string.IsNullOrWhiteSpace(username) &&
                !string.IsNullOrWhiteSpace(password) &&
                !string.IsNullOrWhiteSpace(passCode) &&
                !string.IsNullOrWhiteSpace(phoneInfo))
            {
                await Api.RegisterAsync(new RegisterData
                {
                    NickName = username,
                    Code = passCode,
                    PasswordMd5 = password.ToMd5(),
                    PhoneInfo = phoneInfo
                });
            }
        }

        public override async Task RefreshUserInfoAsync()
        {
            var result = await Api.GetUserInfoAsync();
            if (!result.Success) return;
            Uuid = result.Result["uuid"].ToObject<long>();
            Username = result.Result["name"].ToObject<string>();
            Email = result.Result["email"].ToObject<string>();
            Phone = result.Result["phone"].ToObject<string>();
            TotalSpace = result.Result["spaceCapacity"].ToObject<long>();
            UsedSpace = result.Result["spaceUsed"].ToObject<long>();

        }

        public override Task UploadAsync(FileLocator from, INetDiskFile to, Action<TransferItem> callback)
        {
            throw new NotImplementedException();
        }


        private TransferItem MockDownload(INetDiskFile file, FileLocator to)
        {
            var downloader = FileTransferService
                .GetDownloaderBuilder()
                .UseSixCloudConfigure()
                .From(file.Path)
                .To(to)
                .Build();

            return new TransferItem(downloader)
            {
                File = file,
                Owner = this
            };
        }
    }
}

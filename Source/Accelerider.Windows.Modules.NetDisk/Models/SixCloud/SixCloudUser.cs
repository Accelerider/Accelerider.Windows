using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Modules.NetDisk.Models.OneDrive;
using Accelerider.Windows.TransferService;
using Newtonsoft.Json;
using Refit;

namespace Accelerider.Windows.Modules.NetDisk.Models.SixCloud
{
    public class SixCloudUser : NetDiskUserBase
    {
        [JsonProperty]
        public string AccessToken { get; private set; }

        [JsonProperty]
        public ISixCloudApi Api { get; private set; }

        #region Userinfo

        [JsonProperty]
        public long Uuid { get; private set; }

        [JsonProperty]
        public string Email { get; private set; }

        [JsonProperty]
        public string Phone { get; private set; }

        #endregion

        public SixCloudUser()
        {
            Avatar = new Uri("pack://application:,,,/Accelerider.Windows.Modules.NetDisk;component/Images/logo-six-cloud.png");
            Api = RestService.For<ISixCloudApi>(
                new HttpClient(new AuthenticatedHttpClientHandler(() => AccessToken)) { BaseAddress = new Uri("https://api.6pan.cn") },
                new RefitSettings { JsonSerializerSettings = new JsonSerializerSettings() }
            );
        }

        public override IDownloadingFile Download(INetDiskFile from, FileLocator to)
        {
            return from.ToDownloadingFile(this, builder => builder
                .UseSixCloudConfigure()
                .From(new SixCloudRemotePathProvider((SixCloudFile)from))
                .To(Path.Combine(to, from.Path.FileName)));
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
                    result = json.Result["list"].Select(item => item.ToObject<SixCloudFile>()).ToList();
                    result.ForEach(item => item.Owner = this);
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

        public async Task<bool> LoginAsync(string value, string password)
        {
            if (!string.IsNullOrWhiteSpace(value) &&
                !string.IsNullOrWhiteSpace(password))
            {
                var result = await Api.LoginAsync(new LoginArgs
                {
                    Value = value,
                    Password = password.ToMd5()
                }).RunApi();

                if (result.Success) AccessToken = result.Token;

                return result.Success;
            }

            return false;
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

        public override async Task<bool> RefreshAsync()
        {
            var result = await Api.GetUserInfoAsync().RunApi();
            if (!result.Success) return false;

            Uuid = result.Result["uuid"].ToObject<long>();
            Username = result.Result["name"].ToObject<string>();
            Email = result.Result["email"].ToObject<string>();
            Phone = result.Result["phone"].ToObject<string>();
            Capacity = (result.Result["spaceUsed"].ToObject<long>(), result.Result["spaceCapacity"].ToObject<long>());

            return true;
        }
    }
}

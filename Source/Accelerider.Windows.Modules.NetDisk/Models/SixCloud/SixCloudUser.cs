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
        private readonly List<IDownloadingFile> _downloadingFiles = new List<IDownloadingFile>();

        [JsonProperty]
        private List<string> ArddFilePaths { get; set; } = new List<string>();

        [JsonProperty]
        public string AccessToken { get; private set; }

        [JsonProperty]
        public ISixCloudApi WebApi { get; private set; }

        #region User info

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
            WebApi = RestService.For<ISixCloudApi>(
                new HttpClient(new AuthenticatedHttpClientHandler(() => AccessToken)) { BaseAddress = new Uri("https://api.6pan.cn") },
                new RefitSettings { JsonSerializerSettings = new JsonSerializerSettings() }
            );
        }

        public override IDownloadingFile Download(INetDiskFile from, FileLocator to)
        {
            var downloader = FileTransferService
                .GetDownloaderBuilder()
                .UseSixCloudConfigure()
                .From(new RemotePathProvider(this, from.Path))
                .To(Path.Combine(to, from.Path.FileName))
                .Build();

            var result = DownloadingFile.Create(this, from, downloader);

            // Save download info
            _downloadingFiles.Add(result);
            var savePath = Path.Combine($"{Path.Combine(to, downloader.Context.LocalPath)}{ArddFileExtension}");
            ArddFilePaths.Add(savePath);
            File.WriteAllText(savePath, result.ToJsonString());

            return result;
        }

        public override IReadOnlyList<IDownloadingFile> GetDownloadingFiles()
        {
            return ArddFilePaths
                .Where(item => item.EndsWith(ArddFileExtension))
                .Where(File.Exists)
                .Where(item => !_downloadingFiles.Any(file => item.StartsWith(file.DownloadInfo.Context.LocalPath)))
                .Select(File.ReadAllText)
                .Select(item => item.ToDownloadingFile(builder => builder
                    .UseSixCloudConfigure()
                    .Configure<RemotePathProvider>(provider =>
                    {
                        provider.Owner = this;
                        return provider;
                    }), this))
                .Concat(_downloadingFiles)
                .ToList()
                .AsReadOnly();
        }

        public override IReadOnlyList<ILocalDiskFile> GetDownloadedFiles()
        {
            throw new NotImplementedException();
        }

        public override Task<ILazyTreeNode<INetDiskFile>> GetFileRootAsync()
        {
            var tree = new LazyTreeNode<INetDiskFile>(new SixCloudFile())
            {
                ChildrenProvider = async parent =>
                {
                    var json = await WebApi.GetFilesByPathAsync(new GetFilesByPathArgs { Path = parent.Path, PageSize = 999 });

                    return json.Success
                        ? json.Result["list"].Select(item => item.ToObject<SixCloudFile>()).ToList()
                        : Enumerable.Empty<SixCloudFile>();
                }
            };

            return Task.FromResult<ILazyTreeNode<INetDiskFile>>(tree);
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

        public override async Task<bool> RefreshAsync()
        {
            var result = await WebApi.GetUserInfoAsync().RunApi();
            if (!result.Success) return false;

            Uuid = result.Result["uuid"].ToObject<long>();
            Username = result.Result["name"].ToObject<string>();
            Email = result.Result["email"].ToObject<string>();
            Phone = result.Result["phone"].ToObject<string>();
            Capacity = (result.Result["spaceUsed"].ToObject<long>(), result.Result["spaceCapacity"].ToObject<long>());

            return true;
        }

        public async Task<bool> LoginAsync(string value, string password)
        {
            if (!string.IsNullOrWhiteSpace(value) &&
                !string.IsNullOrWhiteSpace(password))
            {
                var result = await WebApi.LoginAsync(new LoginArgs
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
            var result = await WebApi.SendRegisterMessageAsync(new PhoneArgs() { PhoneNumber = phoneNumber });
            return result.Success ? result.Result.ToObject<string>() : null;
        }

        public async Task SignUpAsync(string username, string password, string passCode, string phoneInfo)
        {
            if (!string.IsNullOrWhiteSpace(username) &&
                !string.IsNullOrWhiteSpace(password) &&
                !string.IsNullOrWhiteSpace(passCode) &&
                !string.IsNullOrWhiteSpace(phoneInfo))
            {
                await WebApi.RegisterAsync(new RegisterData
                {
                    NickName = username,
                    Code = passCode,
                    PasswordMd5 = password.ToMd5(),
                    PhoneInfo = phoneInfo
                });
            }
        }

        private class RemotePathProvider : ConstantRemotePathProvider
        {
            public SixCloudUser Owner { private get; set; }

            private readonly string _filePath;


            public RemotePathProvider(SixCloudUser owner, string filePath)
                : base(new HashSet<string>())
            {
                Owner = owner;
                _filePath = filePath;
            }


            public override async Task<string> GetAsync()
            {
                var path = (await Owner.WebApi.GetFileInfoByPathAsync(new PathArgs { Path = _filePath }))
                    .Result["downloadAddress"]
                    .ToObject<string>();

                if (path == null) return await base.GetAsync();

                RemotePaths.TryAdd(path, 0);

                return path;
            }

            public override IPersister<IRemotePathProvider> GetPersister()
            {
                return new Persister(_filePath);
            }

            private class Persister : IPersister<RemotePathProvider>
            {
                [JsonProperty]
                public string Path { get; }

                public Persister(string path) => Path = path;

                public RemotePathProvider Restore()
                {
                    return new RemotePathProvider(null, Path);
                }
            }
        }
    }
}

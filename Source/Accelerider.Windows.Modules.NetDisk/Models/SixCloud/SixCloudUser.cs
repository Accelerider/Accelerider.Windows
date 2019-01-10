using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.TransferService;
using log4net;
using Newtonsoft.Json;
using Refit;

namespace Accelerider.Windows.Modules.NetDisk.Models.SixCloud
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SixCloudUser : NetDiskUserBase
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SixCloudUser));

        private readonly List<IDownloadingFile> _downloadingFiles = new List<IDownloadingFile>();

        [JsonProperty("LocalDiskFiles")]
        private List<ILocalDiskFile> _localDiskFiles = new List<ILocalDiskFile>();

        [JsonProperty]
        private List<string> ArddFilePaths { get; set; } = new List<string>();

        [JsonProperty]
        public string AccessToken { get; private set; }

        public ISixCloudApi WebApi { get; private set; }

        #region User info

        public string Email { get; private set; }

        public string Phone { get; private set; }

        #endregion

        public SixCloudUser()
        {
            Avatar = new Uri("pack://application:,,,/Accelerider.Windows.Modules.NetDisk;component/Images/logo-sixcloud.png");
            WebApi = RestService.For<ISixCloudApi>(
                "https://api.6pan.cn",
                new RefitSettings
                {
                    JsonSerializerSettings = new JsonSerializerSettings(),
                    AuthorizationHeaderValueGetter = () => Task.FromResult(AccessToken)
                }
            );

            Logger.Info($"The user ({GetHashCode()}-{Username ?? "<Empty>"}) has been created. ");
        }

        public override IDownloadingFile Download(INetDiskFile from, FileLocator to)
        {
            var downloader = FileTransferService
                .GetDownloaderBuilder()
                .UseSixCloudConfigure()
                .Configure(localPath => localPath.GetUniqueLocalPath(path => File.Exists(path) || File.Exists($"{path}{ArddFileExtension}")))
                .From(new RemotePathProvider(this, from.Path))
                .To(Path.Combine(to, from.Path.FileName))
                .Build();

            var result = DownloadingFile.Create(this, from, downloader);

            SaveDownloadingFile(result);

            Logger.Info($"Download: from {from.Path.FileName} to {downloader.Context.LocalPath}. ");

            return result;
        }

        public override IReadOnlyList<IDownloadingFile> GetDownloadingFiles()
        {
            var downloadingFiles = ArddFilePaths
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
                .ToArray();

            ArddFilePaths.Clear();

            downloadingFiles.ForEach(SaveDownloadingFile);

            return _downloadingFiles;
        }

        private void SaveDownloadingFile(IDownloadingFile result)
        {
            _downloadingFiles.Add(result);
            ArddFilePaths.Add(result.ArddFilePath);

            result.DownloadInfo
                .Where(item => item.Status == TransferStatus.Disposed)
                .Subscribe(async _ =>
                {
                    ArddFilePaths.Remove(result.ArddFilePath);

                    await result.ArddFilePath.TryDeleteAsync();
                    await result.DownloadInfo.Context.LocalPath.TryDeleteAsync();

                    Logger.Info($"Cancel Download: {result.DownloadInfo.Context.LocalPath}. ");
                }, OnCompleted);
            Subscribe(result.DownloadInfo.Where(item => item.Status == TransferStatus.Suspended || item.Status == TransferStatus.Faulted));
            Subscribe(result.DownloadInfo.Sample(TimeSpan.FromMilliseconds(5000)));

            File.WriteAllText(result.ArddFilePath, result.ToJsonString());

            if (result.DownloadInfo.Status == TransferStatus.Completed)
            {
                OnCompleted();
            }

            void Subscribe(IObservable<TransferNotification> observable)
            {
                observable.Subscribe(_ => File.WriteAllText(result.ArddFilePath, result.ToJsonString()));
            }

            async void OnCompleted()
            {
                ArddFilePaths.Remove(result.ArddFilePath);

                var localDiskFile = LocalDiskFile.Create(result);
                _downloadingFiles.Remove(result);
                _localDiskFiles.Add(localDiskFile);

                await result.ArddFilePath.TryDeleteAsync();
            }
        }

        public override IReadOnlyList<ILocalDiskFile> GetDownloadedFiles()
        {
            return _localDiskFiles;
        }

        public override void ClearDownloadFiles()
        {
            _localDiskFiles.Clear();
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

            Id = result.Result["uuid"].ToObject<long>().ToString();
            Username = result.Result["name"].ToObject<string>();
            Email = result.Result["email"].ToObject<string>();
            Phone = result.Result["phone"].ToObject<string>();
            UsedCapacity = result.Result["spaceUsed"].ToObject<long>();
            TotalCapacity = result.Result["spaceCapacity"].ToObject<long>();

            return true;
        }

        public async Task<bool> LoginAsync(string account, string password)
        {
            if (!string.IsNullOrWhiteSpace(account) &&
                !string.IsNullOrWhiteSpace(password))
            {
                var result = await WebApi.LoginAsync(new LoginArgs
                {
                    Value = account,
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
                await WebApi.RegisterAsync(new RegisterArgs
                {
                    NickName = username,
                    PhoneCode = passCode,
                    PasswordMd5 = password.ToMd5(),
                    PhoneInfo = phoneInfo
                });
            }
        }

        private class RemotePathProvider : IRemotePathProvider
        {
            [JsonIgnore]
            public SixCloudUser Owner { private get; set; }

            [JsonProperty("Path")]
            private readonly string _filePath;

            [JsonConstructor]
            public RemotePathProvider() { }

            public RemotePathProvider(SixCloudUser owner, string filePath)
            {
                Owner = owner;
                _filePath = filePath;
            }

            public async Task<string> GetAsync()
            {
                var path = (await Owner.WebApi.GetFileInfoByPathAsync(new PathArgs { Path = _filePath }))
                    .Result["downloadAddress"]
                    .ToObject<string>();

                if (path == null) throw new RemotePathExhaustedException(this);

                return path;
            }

            public void Rate(string remotePath, double score) { }
        }
    }
}

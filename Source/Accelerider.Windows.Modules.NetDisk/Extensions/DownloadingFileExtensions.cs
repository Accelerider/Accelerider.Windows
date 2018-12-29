using System;
using Accelerider.Windows.Modules.NetDisk.Models;
using Accelerider.Windows.TransferService;
using Newtonsoft.Json.Linq;

namespace Accelerider.Windows.Modules.NetDisk
{
    public static class DownloadingFileExtensions
    {
        private class SerializedData
        {
            public INetDiskFile File { get; set; }

            public JObject DownloadInfo { get; set; }
        }

        public static string ToJsonString(this IDownloadingFile @this)
        {
            return new SerializedData
            {
                File = @this.File,
                DownloadInfo = @this.DownloadInfo.ToJObject()
            }.ToJson();
        }

        public static IDownloadingFile ToDownloadingFile(this string jsonText, Action<IDownloaderBuilder> configure, INetDiskUser owner)
        {
            var data = jsonText.ToObject<SerializedData>();

            var file = data.File;
            var builder = FileTransferService.GetDownloaderBuilder();
            configure(builder);
            var downloader = builder.Build(data.DownloadInfo.ToString());

            return DownloadingFile.Create(owner, file, downloader);
        }
    }
}

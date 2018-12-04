using System;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.TransferService.WpfInteractions;
using Accelerider.Windows.Modules.NetDisk.Enumerations;
using Accelerider.Windows.Modules.NetDisk.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Accelerider.Windows.Modules.NetDisk.Models.OneDrive
{
    [JsonObject(MemberSerialization.OptIn)]
    public class OneDriveFile : INetDiskFile
    {
        public FileType Type => _folder != null ? FileType.FolderType : FileType.OtherType;

        public FileLocator Path => _pathInfo?.Value<string>("path").Replace("/drive/root:", string.Empty) ?? "/";

        [JsonProperty("size")]
        public DisplayDataSize Size { get; set; }

        [JsonProperty("lastModifiedDateTime")]
        public DateTime ModifiedTime { get; set; }

        [JsonProperty("file")]
        private JToken _file;

        [JsonProperty("folder")]
        private JToken _folder;

        [JsonProperty("parentReference")]
        private JToken _pathInfo;



        public OneDriveUser Owner { get; set; }

        public async Task<bool> DeleteAsync()
        {
            return (await Owner.Api.DeleteFileAsync(Path)).Error == null;
        }
    }
}

using System;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;
using Newtonsoft.Json;

namespace Accelerider.Windows.Core.Files
{
    public class SharedFile : ISharedFile
    {
        [JsonProperty("fs_id")]
        public string Title { get; set; }

        [JsonProperty("fs_id")]
        public DateTime SharedTime { get; set; }

        [JsonProperty("fs_id")]
        public Uri ShareLink { get; set; }

        [JsonProperty("fs_id")]
        public string AccessCode { get; set; }

        [JsonProperty("fs_id")]
        public FileTypeEnum FileType { get; set; }

        public Task<bool> DeleteAsync()
        {
            throw new NotImplementedException();
        }
    }
}

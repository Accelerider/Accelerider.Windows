using System.IO;
using BaiduPanDownloadWpf.Infrastructure;
using BaiduPanDownloadWpf.Infrastructure.Interfaces;
using Newtonsoft.Json;

namespace BaiduPanDownloadWpf.Core
{
    public class LocalConfigInfo : ILocalConfigInfo
    {
        private static readonly string FilePath = Path.Combine(Common.UserDataSavePath, "config.json");

        public string Theme { get; set; }
        public string Background { get; set; }
        public LanguageEnum Language { get; set; } = LanguageEnum.Chinese;
        public bool IsDisplayDownloadDialog { get; set; } = true;
        public string DownloadDirectory { get; set; } = Directory.GetDirectoryRoot(Directory.GetCurrentDirectory()) + "BaiduDownload";
        public int ParallelTaskNumber { get; set; } = 1;
        public double SpeedLimit { get; set; } = 16;

        public static LocalConfigInfo Create()
        {
            LocalConfigInfo result;
            if (File.Exists(FilePath))
            {
                var info = File.ReadAllText(FilePath);
                result = JsonConvert.DeserializeObject<LocalConfigInfo>(info);
                if (result != null) return result;
            }
            result = new LocalConfigInfo();
            result.Save();
            return result;
        }
        public void Save()
        {
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(this));
        }

        private LocalConfigInfo() { }
    }
}

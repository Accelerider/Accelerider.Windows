using System.Management;
using System.Text.RegularExpressions;

namespace Accelerider.Windows.Infrastructure
{
    public static class SystemInfo
    {
        private static readonly string _systemInfo = GetSystemInfo();


        public static readonly string Caption = ExtractSystemInfo("Caption");
        public static readonly string CSName = ExtractSystemInfo("CSName");
        public static readonly string InstallDate = ExtractSystemInfo("InstallDate");
        public static readonly string OSArchitecture = ExtractSystemInfo("OSArchitecture");
        public static readonly string SerialNumber = ExtractSystemInfo("SerialNumber");
        public static readonly string Version = ExtractSystemInfo("Version");
        public static readonly string TotalVisibleMemorySize = ExtractSystemInfo("TotalVisibleMemorySize");


        private static string GetSystemInfo()
        {
            var result = new ManagementObjectSearcher(new ObjectQuery("SELECT * FROM Win32_OperatingSystem"));
            var strInfo = string.Empty;
            using (var collection = result.Get())
            {
                foreach (var item in collection)
                {
                    var managementObject = item as ManagementObject;
                    strInfo = managementObject?.GetText(TextFormat.Mof);
                    if (!string.IsNullOrEmpty(strInfo)) break;
                }
            }
            return strInfo;
        }

        private static string ExtractSystemInfo(string infoKey) => 
            Regex.Match(_systemInfo, $"{infoKey} ?= ?\"(.+?)\";").Groups[1].Value;
    }
}

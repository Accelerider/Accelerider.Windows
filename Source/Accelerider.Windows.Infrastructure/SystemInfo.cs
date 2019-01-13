using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;
using log4net;

namespace Accelerider.Windows.Infrastructure
{
    public static class SystemInfo
    {
        private const string ExtractionFailed = "<Extraction Failed>";

        private static readonly ILog Logger = LogManager.GetLogger(typeof(SystemInfo));

        public static readonly string Caption;
        public static readonly string CSName;
        public static readonly string InstallDate;
        public static readonly string OSArchitecture;
        public static readonly string SerialNumber;
        public static readonly string Version;
        public static readonly long TotalVisibleMemorySize;
        public static readonly string CPUName;

        static SystemInfo()
        {
            var extractionRegex = new Regex("(\\w+?) ?= ?\"(.+?)\";", RegexOptions.Compiled | RegexOptions.Multiline);

            var systemInfo = GetManagementInfo("SELECT * FROM Win32_OperatingSystem", extractionRegex);
            var processorInfo = GetManagementInfo("SELECT * FROM Win32_Processor", extractionRegex);

            Caption = systemInfo.GetValue("Caption");
            CSName = systemInfo.GetValue("CSName");
            InstallDate = systemInfo.GetValue("InstallDate");
            OSArchitecture = systemInfo.GetValue("OSArchitecture");
            SerialNumber = systemInfo.GetValue("SerialNumber");
            Version = systemInfo.GetValue("Version");
            TotalVisibleMemorySize = systemInfo.GetValue("TotalVisibleMemorySize").CastTo<long>();

            CPUName = processorInfo.GetValue("Name");
        }

        private static IReadOnlyDictionary<string, string> GetManagementInfo(string query, Regex regex)
        {
            try
            {
                var result = new ManagementObjectSearcher(new ObjectQuery(query));
                var infoString = string.Empty;
                using (var collection = result.Get())
                {
                    foreach (var item in collection)
                    {
                        var managementObject = item as ManagementObject;
                        infoString = managementObject?.GetText(TextFormat.Mof);
                        if (!string.IsNullOrEmpty(infoString)) break;
                    }
                }

                if (string.IsNullOrEmpty(infoString)) return null;

                return regex.Matches(infoString).OfType<Match>()
                    .Where(item => item.Success)
                    .ToDictionary(item => item.Groups[1].Value, item => item.Groups[2].Value);
            }
            catch (Exception e)
            {
                Logger.Error("An unexpected exception occured while getting management info. ", e);

                return null;
            }
        }

        private static string GetValue(this IReadOnlyDictionary<string, string> info, string key)
        {
            return info != null && info.TryGetValue(key, out var value) ? value : ExtractionFailed;
        }
    }
}

using System;
using System.Collections.Generic;

namespace Accelerider.Windows.Infrastructure.Upgrade
{
    public class UpgradeInfo
    {
        public Version Version { get; }

        public List<(string FileName, string Url)> PrivateFiles { get; }

        public List<(string FileName, string Url)> PublicFiles { get; }

        public UpgradeInfo(Version version, List<(string FileName, string Url)> privateFiles, List<(string FileName, string Url)> publicFiles)
        {
            Version = version;
            PrivateFiles = privateFiles;
            PublicFiles = publicFiles;
        }
    }
}

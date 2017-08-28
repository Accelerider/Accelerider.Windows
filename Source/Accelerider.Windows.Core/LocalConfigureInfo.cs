using System;
using System.Collections.Generic;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Core
{
    internal class LocalConfigureInfo : ILocalConfigureInfo
    {
        public string Theme { get; set; }
        public LanguageEnum Language { get; set; }


        public string Username { get; set; } = "name";
        public string PasswordEncrypted { get; set; } = "";
        public bool IsAutoSignIn { get; set; }


        public bool NotDisplayDownloadDialog { get; set; }
        public FileLocation DownloadDirectory { get; set; }
        public int ParallelTaskNumber { get; set; }
        public double SpeedLimit { get; set; }


        public void Save()
        {
            
        }
    }
}
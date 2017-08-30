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


        public string Username { get; set; } = "test_2017";
        public string PasswordEncrypted { get; set; } = "test_2017";
        public bool IsAutoSignIn { get; set; } = true;


        public bool NotDisplayDownloadDialog { get; set; }
        public FileLocation DownloadDirectory { get; set; }
        public int ParallelTaskNumber { get; set; }
        public double SpeedLimit { get; set; }


        public void Save()
        {
            
        }
    }
}
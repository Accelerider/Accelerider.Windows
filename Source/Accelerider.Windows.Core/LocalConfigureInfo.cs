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


        public string Username { get; set; } = "Test_89757";
        public string PasswordEncrypted { get; set; } = "12345678123456781234567812345678";
        public bool IsAutoSignIn { get; set; }


        public bool IsDisplayDownloadDialog { get; set; }
        public string DownloadDirectory { get; set; }
        public int ParallelTaskNumber { get; set; }
        public double SpeedLimit { get; set; }


        public void Save()
        {
            
        }
    }
}
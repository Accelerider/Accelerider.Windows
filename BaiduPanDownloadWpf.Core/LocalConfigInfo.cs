using BaiduPanDownloadWpf.Infrastructure.Interfaces.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaiduPanDownloadWpf.Infrastructure.Interfaces;
using BaiduPanDownloadWpf.Infrastructure;

namespace BaiduPanDownloadWpf.Core
{
    public class LocalConfigInfo : ILocalConfigInfo
    {
        public string Background { get; set; }

        public string DownloadPath { get; set; }

        public bool IsDisplayDownloadDialog { get; set; }

        public LanguageEnum Language { get; set; }

        public int ParellelTaskNumber { get; set; }

        public string Theme { get; set; }

        public int ThreadNumber { get; set; }

    }
}

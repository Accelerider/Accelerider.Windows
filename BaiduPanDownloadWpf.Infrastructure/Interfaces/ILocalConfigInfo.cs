using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPanDownloadWpf.Infrastructure.Interfaces
{
    public interface ILocalConfigInfo
    {
        int ThreadNumber { get; set; }
        int ParellelTaskNumber { get; set; }
        string DownloadPath { get; set; }
        LanguageEnum Language { get; set; }
        string Theme { get; set; }
        string Background { get; set; }
        bool IsDisplayDownloadDialog { get; set; }
    }
}

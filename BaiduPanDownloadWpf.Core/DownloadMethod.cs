using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPanDownloadWpf.Core
{
    /// <summary>
    /// 下载方法
    /// </summary>
    public enum DownloadMethod
    {
        /// <summary>
        /// 直接下载
        /// </summary>
        DirectDownload = 1,
        /// <summary>
        /// 中转下载
        /// </summary>
        JumpDownload = 2,
        /// <summary>
        /// APPID下载
        /// </summary>
        AppidDownload = 3
    }
}

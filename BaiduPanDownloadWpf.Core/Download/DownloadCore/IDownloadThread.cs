using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPanDownloadWpf.Core.Download.DownloadCore
{
    internal interface IDownloadThread
    {
        #region
        int ID { get; set; }
        /// <summary>
        /// 下载链接
        /// </summary>
        string DownloadUrl { get; set; }
        /// <summary>
        /// 下载路径
        /// </summary>
        string Path { get; set; }
        /// <summary>
        /// 下载块信息
        /// </summary>
        DownloadBlock Block { get; set; }
        /// <summary>
        /// 下载信息
        /// </summary>
        DownloadInfo Info { get; set; }
        #endregion

        event Action ThreadCompletedEvent;

        void Stop();

        void Restart();
    }
}

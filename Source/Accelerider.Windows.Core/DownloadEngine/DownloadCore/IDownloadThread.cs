using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerider.Windows.Core.DownloadEngine.DownloadCore
{
    internal interface IDownloadThread
    {
        #region
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

        event Action<IDownloadThread> ThreadCompletedEvent;

        void Stop();

        void Restart();

        void ForceStop();
    }
}

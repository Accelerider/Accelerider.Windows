using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Accelerider.Windows.Core.DownloadEngine.DownloadCore.DownloadThread;
using Accelerider.Windows.Core.NetWork;
using Accelerider.Windows.Core.Tools;
using Accelerider.Windows.Infrastructure;

namespace Accelerider.Windows.Core.DownloadEngine.DownloadCore
{
    internal class HttpDownload
    {
        #region 参数

        /// <summary>
        /// 下载地址(支持多URL下载)
        /// </summary>
        public string[] Url => Info.DownloadUrl;

        /// <summary>
        /// 下载路径
        /// </summary>
        public string DownloadPath => Info.DownloadPath;

        /// <summary>
        /// 线程数
        /// </summary>
        public int ThreadNum
        {
            get => Info.ThreadNum;
            set => Info.ThreadNum = value;
        }

        /// <summary>
        /// 下载速度
        /// </summary>
        public long DownloadSpeed { get; private set; }

        public Dictionary<string, string> Data => Info.Data;

        /// <summary>
        /// 下载进度
        /// </summary>
        public float DownloadPercentage { get; private set; }

        /// <summary>
        /// 下载状态
        /// </summary>
        public TransferTaskStatusEnum DownloadState
        {
            get => _state;
            set
            {
                if (_state != value)
                {
                    var temp = _state;
                    _state = value;
                    DownloadStateChangedEvent?.Invoke(this, new StateChangedArgs(temp, _state));
                }
            }
        }


        /// <summary>
        /// 下载所使用的的Cookies
        /// </summary>
        public Cookies UserCookies => Info.UserCookies;

        /// <summary>
        /// Cookies所使用的的Domain
        /// </summary>
        public string Domain => Info.Domain;

        /// <summary>
        /// 下载所使用的的Referer
        /// </summary>
        public string Referer => Info.Referer;

        /// <summary>
        /// 下载所使用的的UA
        /// </summary>
        public string UserAgent => Info.UserAgent;

        public DownloadInfo Info { get; set; }


        public List<IDownloadThread> Threads { get; private set; }

        #endregion

        #region 事件
        public delegate void DownloadStateChanged(object sender, StateChangedArgs args);
        public delegate void DownloadPercentageChanged(object sender, PercentageChangedEventArgs args);
        /// <summary>
        /// 下载状态改变事件
        /// </summary>
        public event DownloadStateChanged DownloadStateChangedEvent;
        /// <summary>
        /// 下载进度改变事件
        /// </summary>
        public event DownloadPercentageChanged DownloadPercentageChangedEvent;
        #endregion

        #region 私有参数
        private float _percentage = -1F;
        private long _speed = 0L;
        private TransferTaskStatusEnum _state = TransferTaskStatusEnum.Waiting;

        #endregion
        /// <summary>
        /// 开始下载
        /// </summary>
        public void Start()
        {
            if (Info == null)
            {
                throw new NullReferenceException("参数错误");
            }
            try
            {

                if (Info.Completed)
                {
                    DownloadState = TransferTaskStatusEnum.Completed;
                    DownloadPercentage = 100F;
                    DownloadSpeed = 0;
                    return;
                }
                DownloadState = TransferTaskStatusEnum.Transferring;
                var response = GetResponse(Url);
                if (response == null)
                {
                    DownloadState = TransferTaskStatusEnum.Faulted;
                    return;
                }
                if (!File.Exists(DownloadPath))
                {
                    var stream = new FileStream(DownloadPath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite, 1024 * 1024 * 5);
                    stream.SetLength(response.ContentLength);
                    stream.Close();
                }
                Threads?.ForEach(v => v.ForceStop());
                Threads = new List<IDownloadThread>();
                new Thread(ReportDownloadProgress).Start();
                var num = 0;
                for (var i = 0; i < Info.ThreadNum; i++)
                {
                    var block = Info.DownloadBlockList.FirstOrDefault(v => !v.Completed && !v.Downloading);
                    if (block == null) break;
                    if (num == Url.Length)
                        num = 0;
                    var url = Url[num];
                    if (Info.DownloadCount.Count > 0)
                        url = Info.DownloadCount.FirstOrDefault(v => v.Value == Info.DownloadCount.Values.Max()).Key;
                    var thread = new CommonDownloadThread(url, DownloadPath, block, Info);
                    thread.ThreadCompletedEvent += HttpDownload_ThreadCompletedEvent;
                    Threads.Add(thread);
                    num++;
                }
            }
            catch
            {
                DownloadState = TransferTaskStatusEnum.Faulted;
            }
        }

        private int _num = 0;
        private void HttpDownload_ThreadCompletedEvent(IDownloadThread downloadThread)
        {
            lock (this)
            {
                Threads.Remove(downloadThread);
                if (Info.DownloadBlockList.All(v => v.Completed))
                {
                    DownloadSpeed = 0L;
                    DownloadPercentage = 100F;
                    Info.Completed = true;
                    Info.CompletedTime = DateTime.Now;
                    DownloadState = TransferTaskStatusEnum.Checking;
                    return;
                }
                var block = Info.DownloadBlockList.FirstOrDefault(v => !v.Completed && !v.Downloading);
                if (block == null) return;
                _num++;
                if (_num == Url.Length)
                    _num = 0;
                var url = Url[_num];
                if (Info.DownloadCount.Count > 0)
                    url = Info.DownloadCount.FirstOrDefault(v => v.Value == Info.DownloadCount.Values.Max()).Key;
                var thread = new CommonDownloadThread(url, DownloadPath, block, Info);
                thread.ThreadCompletedEvent += HttpDownload_ThreadCompletedEvent;
                Threads.Add(thread);
            }
        }

        /// <summary>
        /// 获取下载任务
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static HttpDownload GetTaskByInfo(DownloadInfo info)
        {
            return new HttpDownload()
            {
                Info = info
            };
        }

        /// <summary>
        /// 创建任务数据
        /// </summary>
        /// <param name="urls"></param>
        /// <param name="downloadPath"></param>
        /// <param name="threadNum"></param>
        /// <param name="cookies"></param>
        /// <param name="data"></param>
        /// <param name="blockSize"></param>
        /// <returns></returns>
        public static DownloadInfo CreateTaskInfo(string[] urls, string downloadPath, int threadNum, Dictionary<string, string> data = null, Cookies cookies = null, long blockSize = 1024 * 1024 * 10)
        {
            var response = GetResponse(urls);
            if (response == null)
                return null;
            var info = new DownloadInfo()
            {
                ContentLength = response.ContentLength,
                DownloadUrl = urls,
                BlockLength = response.ContentLength < blockSize
                    ? response.ContentLength < 100
                        ? response.ContentLength
                        : blockSize / threadNum
                    : blockSize,
                DownloadPath = downloadPath,
                ThreadNum = threadNum,
            };
            info.Init(data);
            return info;
        }

        private void ReportDownloadProgress()
        {
            var temp = 0L;
            while (DownloadState == TransferTaskStatusEnum.Transferring)
            {
                Thread.Sleep(1000);
                if (temp == 0)
                {
                    temp = Info.CompletedLength;
                }
                else
                {
                    if (DownloadState == TransferTaskStatusEnum.Transferring)
                    {
                        DownloadSpeed = Info.CompletedLength - temp;
                        DownloadPercentage = Info.CompletedLength;
                        DownloadPercentageChangedEvent?.Invoke(this, new PercentageChangedEventArgs(DownloadPercentage, DownloadSpeed));
                        temp = Info.CompletedLength;
                    }
                }
            }
        }
        /// <summary>
        /// 保存并结束
        /// </summary>
        public DownloadInfo StopAndSave()
        {
            if (Threads != null)
            {
                foreach (var thread in Threads)
                    thread.Stop();
                DownloadState = TransferTaskStatusEnum.Paused;
                return Info;
            }
            return null;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is HttpDownload))
                return false;
            var download = (HttpDownload)obj;
            return download.DownloadPath == DownloadPath;
        }

        public override int GetHashCode()
        {
            return DownloadPath.GetHashCode();
        }

        private static HttpWebResponse GetResponse(string[] urls)
        {
            foreach (var sub in urls)
            {
                try
                {
                    var request = WebRequest.Create(sub) as HttpWebRequest;
                    request.UserAgent = "netdisk;5.5.2.0;PC;PC-Windows;6.1.7601;WindowsBaiduNetdisk";
                    request.Timeout = 10000;
                    return (HttpWebResponse)request.GetResponse();
                }
                catch (WebException ex)
                {
                    if (ex.Message.Contains("超时") || ex.Message.Contains("Timeout"))
                    {
                        return GetResponse(urls);
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }


    }
    internal class PercentageChangedEventArgs : EventArgs
    {
        public float Progress { get; }
        public long Speed { get; }

        public PercentageChangedEventArgs(float progress, long speed)
        {
            this.Progress = progress;
            this.Speed = speed;
        }
    }

    internal class StateChangedArgs : EventArgs
    {
        public TransferTaskStatusEnum OldState { get; }
        public TransferTaskStatusEnum NewState { get; }

        public StateChangedArgs(TransferTaskStatusEnum oldState, TransferTaskStatusEnum newState)
        {
            this.OldState = oldState;
            this.NewState = newState;
        }
    }
}

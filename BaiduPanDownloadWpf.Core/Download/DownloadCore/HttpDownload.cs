using BaiduPanDownloadWpf.Core.NetWork;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using BaiduPanDownloadWpf.Core.Download.DownloadCore;
using BaiduPanDownloadWpf.Infrastructure;


namespace BaiduPanDownloadWpf.Core.Download.DwonloadCore
{
    /// <summary>
    /// HTTP下载
    /// </summary>
    public class HttpDownload
    {
        #region 参数

        /// <summary>
        /// 下载地址(支持多URL下载)
        /// </summary>
        public string[] Url { get; set; }

        /// <summary>
        /// 下载路径
        /// </summary>
        public string DownloadPath { get; set; }

        /// <summary>
        /// 线程数
        /// </summary>
        public int ThreadNum { get; set; }

        /// <summary>
        /// 下载速度
        /// </summary>
        public long DownloadSpeed { get; set; }

        /// <summary>
        /// 下载进度
        /// </summary>
        public float DownloadPercentage { get; set; }

        /// <summary>
        /// 下载状态
        /// </summary>
        public DownloadStateEnum DownloadState
        {
            get { return _state; }
            private set
            {
                if (_state != value)
                {
                    DownloadStateChangedEvent?.Invoke(this, new StateChangedArgs(_state, value));
                    _state = value;
                }
            }
        }

        /// <summary>
        /// 下载所使用的的Cookies
        /// </summary>
        public Cookies UserCookies { get; set; }

        /// <summary>
        /// Cookies所使用的的Domain
        /// </summary>
        public string Domain { get; set; } = ".baidu.com";

        /// <summary>
        /// 下载所使用的的Referer
        /// </summary>
        public string Referer { get; set; } = "http://pan.baidu.com/disk/home";

        /// <summary>
        /// 下载所使用的的UA
        /// </summary>
        public string UserAgent { get; set; } = "netdisk;5.5.2.0;PC;PC-Windows;6.1.7601;WindowsBaiduNetdisk";

        public DownloadInfo Info { get; set; }
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
        private DownloadStateEnum _state = DownloadStateEnum.Waiting;
        private DownloadThread[] _threads;
        #endregion

        /// <summary>
        /// 开始下载
        /// </summary>
        public void Start()
        {

            if (Url == null || Url.Length == 0 || ThreadNum < 1 || Info == null)
            {
                throw new NullReferenceException("参数错误");
            }
            try
            {
                
                if (Info.Completed)
                {
                    DownloadState = DownloadStateEnum.Completed;
                    DownloadPercentage = 100F;
                    DownloadSpeed = 0;
                    return;
                }
                DownloadState = DownloadStateEnum.Downloading;
                var response = GetResponse();
                if (response == null)
                {
                    DownloadState = DownloadStateEnum.Faulted;
                    return;
                }
                if (!File.Exists(DownloadPath))
                {
                    var stream = new FileStream(DownloadPath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite, 1024 * 1024 * 5);
                    stream.SetLength(response.ContentLength);
                    stream.Close();
                }
                _threads = new DownloadThread[Info.DownloadBlockList.Count];
                new Thread(ReportDownloadProgress).Start();
                var num = 0;
                for (var i = 0; i < Info.DownloadBlockList.Count; i++)
                {
                    var block = Info.DownloadBlockList[i];
                    if (num == Url.Length)
                    {
                        num = 0;
                    }
                    _threads[i] = new DownloadThread()
                    {
                        ID = i,
                        DownloadUrl = Url[num],
                        Path = DownloadPath,
                        Block = block,
                        Info = Info
                    };
                    _threads[i].ThreadCompletedEvent += HttpDownload_ThreadCompletedEvent;
                    num++;
                }
            }
            catch
            {
                DownloadState = DownloadStateEnum.Faulted;
            }
        }
        int _completedThread = 0;
        private void HttpDownload_ThreadCompletedEvent()
        {
            lock (this)
            {
                _completedThread++;
                if (_completedThread >= _threads.Length)
                {
                    DownloadSpeed = 0L;
                    DownloadPercentage = 100F;
                    Info.Completed = true;
                    Info.CompletedTime=DateTime.Now;
                    DownloadState = DownloadStateEnum.Completed;
                }
            }
        }

        /// <summary>
        /// 创建数据
        /// </summary>
        public async Task<DownloadInfo> CreateData()
        {
            return await Task.Run(() =>
            {
                var response = GetResponse();
                if (response == null)
                {
                    throw new NullReferenceException("下载链接已失效");
                }
                var info = new DownloadInfo
                {
                    ContentLength = response.ContentLength,
                    BlockLength =
                        response.ContentLength < 1024 ? response.ContentLength : response.ContentLength/ThreadNum,
                    DownloadUrl = Url,
                    DownloadPath = DownloadPath,
                    UserCookies = UserCookies,
                    Domain = Domain,
                    Referer = Referer,
                    UserAgent = UserAgent
                };
                info.init();
                return info;
            });
        }
        /// <summary>
        /// 创建数据(带路径)
        /// </summary>
        public async Task<DownloadInfo> CreateData(string path)
        {
            var info = await CreateData();
            info.init(path);
            return info;
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
                Url = info.DownloadUrl,
                DownloadPath = info.DownloadPath,
                UserCookies = info.UserCookies,
                Domain = info.Domain,
                Referer = info.Referer,
                ThreadNum = info.DownloadBlockList.Count,
                UserAgent = info.UserAgent,
                Info = info
            };
        }

        /// <summary>
        /// 创建任务数据
        /// </summary>
        /// <param name="urls"></param>
        /// <param name="downloadPath"></param>
        /// <param name="thrreadNum"></param>
        /// <param name="cookies"></param>
        /// <returns></returns>
        public static async Task<DownloadInfo> CreateTaskInfo(string[] urls, string downloadPath, int thrreadNum, Cookies cookies = null)
        {
            return await new HttpDownload()
            {
                Url = urls,
                DownloadPath = downloadPath,
                UserCookies = cookies,
                ThreadNum = thrreadNum
            }.CreateData();
        }

        private void ReportDownloadProgress()
        {
            var temp = 0L;
            while (DownloadState == DownloadStateEnum.Downloading)
            {
                Thread.Sleep(1000);
                if (temp == 0)
                {
                    temp = Info.CompletedLength;
                }
                else
                {
                    if (DownloadState == DownloadStateEnum.Downloading)
                    {
                        DownloadSpeed = Info.CompletedLength - temp;
                        DownloadPercentage = Info.CompletedLength;
                        DownloadPercentageChangedEvent?.Invoke(this,new PercentageChangedEventArgs(DownloadPercentage,DownloadSpeed));
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
            if (_threads != null)
            {
                _completedThread = 0;
                DownloadState = DownloadStateEnum.Paused;
                foreach (var thread in _threads)
                {
                    thread.Stop();
                }
                return Info;
            }
            return null;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is HttpDownload))
            {
                return false;
            }
            var download = (HttpDownload) obj;
            return download.DownloadPath == DownloadPath;
        }

        public override int GetHashCode()
        {
            return DownloadPath.GetHashCode();
        }

        private HttpWebResponse GetResponse()
        {
            foreach (var sub in Url)
            {
                try
                {
                    var request = WebRequest.Create(sub) as HttpWebRequest;
                    request.Referer = Referer;
                    request.UserAgent = UserAgent;
                    request.Timeout = 10000;
                    if (UserCookies != null)
                    {
                        request.CookieContainer = new CookieContainer();
                        foreach (var key in UserCookies.GetKeys())
                        {
                            var ck = new Cookie(key, UserCookies.GetCookie(key)) { Domain = Domain };
                            request.CookieContainer.Add(ck);
                        }
                    }
                    return (HttpWebResponse)request.GetResponse();
                }
                catch(WebException ex)
                {
                    if (ex.Message.Contains("超时") || ex.Message.Contains("Timeout"))
                    {
                        return GetResponse();
                    }
                }
            }
            return null;
        }
    }

    public class PercentageChangedEventArgs : EventArgs
    {
        public float Progress { get; }
        public long Speed { get; }

        public PercentageChangedEventArgs(float progress, long speed)
        {
            this.Progress = progress;
            this.Speed = speed;
        }
    }

    public class StateChangedArgs : EventArgs
    {
        public DownloadStateEnum OldState { get; }
        public DownloadStateEnum NewState { get; }

        public StateChangedArgs(DownloadStateEnum oldState, DownloadStateEnum newState)
        {
            this.OldState = oldState;
            this.NewState = newState;
        }
    }
}

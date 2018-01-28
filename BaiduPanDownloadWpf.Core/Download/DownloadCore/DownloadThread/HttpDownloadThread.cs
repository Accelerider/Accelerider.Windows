using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BaiduPanDownloadWpf.Core.Download.DownloadCore.DownloadThread
{
    /// <summary>
    /// 下载线程
    /// </summary>
    public class HttpDownloadThread : IDownloadThread
    {
        #region
        public int ID { get; set; }
        /// <summary>
        /// 下载链接
        /// </summary>
        public string DownloadUrl { get; set; }
        /// <summary>
        /// 下载路径
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 下载块信息
        /// </summary>
        public DownloadBlock Block { get; set; }
        /// <summary>
        /// 下载信息
        /// </summary>
        public DownloadInfo Info { get; set; }
        #endregion
        public event Action ThreadCompletedEvent;

        bool _stoped;
        private bool _rest=false;
        private HttpWebRequest _request;
        private HttpWebResponse _response;

        internal HttpDownloadThread()
        {
            Task.Run(()=>Start());
        }

        private async void Start()
        {
            while (true)
            {
                try
                {
                    await Task.Delay(300);
                    _request?.Abort();
                    _response?.Close();
                    if (_stoped)
                    {
                        return;
                    }
                    if (Block.Completed)
                    {
                        ThreadCompletedEvent?.Invoke();
                        return;
                    }
                    if (_rest)
                    {
                        _rest = false;
                    }
                    _request = WebRequest.Create(DownloadUrl) as HttpWebRequest;
                    _request.UserAgent = Info.UserAgent;
                    _request.Referer = Info.Referer;
                    if (Info.UserCookies != null)
                    {
                        _request.CookieContainer = new CookieContainer();
                        foreach (var Key in Info.UserCookies.GetKeys())
                        {
                            var ck = new Cookie(Key, Info.UserCookies.GetCookie(Key)) { Domain = Info.Domain };
                            _request.CookieContainer.Add(ck);
                        }
                    }
                    _request.Timeout = 8000;
                    _request.ReadWriteTimeout = 8000;
                    _request.AddRange(Block.From, Block.To);
                    _response = _request.GetResponse() as HttpWebResponse;
                    if (!File.Exists(Path))
                    {
                        return;
                    }
                    using (var responseStream = _response.GetResponseStream())
                    {
                        using (var stream = new FileStream(Path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite, 1024 * 1024))
                        {
                            stream.Seek(Block.From, SeekOrigin.Begin);
                            var array = new byte[1024];
                            var i = responseStream.Read(array, 0, array.Length);
                            while (true)
                            {
                                if (_rest || _stoped)
                                    return;
                                if (i <= 0 && Block.From - 1 != Block.To && Block.From != Block.To)
                                {
                                    Restart();
                                    continue;
                                }
                                if (i <= 0)
                                {
                                    break;
                                }
                                stream.Write(array, 0, i);
                                Block.From += i;
                                Block.CompletedLength += i;
                                Info.CompletedLength += i;
                                Info.DownloadBlockList[ID] = Block;
                                i = responseStream.Read(array, 0, array.Length);
                            }
                            Block.Completed = true;
                            ThreadCompletedEvent?.Invoke();
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("终止") || ex.Message.Contains("取消"))
                    {
                        return;
                    }
                    Next();
                }
            }
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            if (Block.Completed)
            {
                return;
            }
            _stoped = true;
        }

        public void Restart()
        {
            _rest = true;
            Task.Run(() => Start());
        }

        private int _num;

        private void Next()
        {
            _num++;
            if (_num == Info.DownloadUrl.Length)
            {
                _num = 0;
            }
            DownloadUrl = Info.DownloadUrl[_num];
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Accelerider.Windows.Core.DownloadEngine.DownloadCore.DownloadThread
{
    /// <summary>
    /// 祖传代码....
    /// </summary>
    internal class CommonDownloadThread : IDownloadThread
    {
        #region
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
        public DownloadBlock Block
        {
            get => _block;
            set
            {
                value.Downloading = true;
                _block = value;
            }
        }
        /// <summary>
        /// 下载信息
        /// </summary>
        public DownloadInfo Info { get; set; }
        #endregion
        public event Action<IDownloadThread> ThreadCompletedEvent;

        bool _stoped;
        private bool _rest = false;
        private HttpWebRequest _request;
        private HttpWebResponse _response;
        private DownloadBlock _block;

        internal CommonDownloadThread(string url, string path, DownloadBlock block, DownloadInfo info)
        {
            DownloadUrl = url;
            Path = path;
            Block = block;
            Info = info;
            new Thread(Start) { IsBackground = true }.Start();
        }

        private void Start()
        {
            try
            {
                if (_stoped) return;
                if (_rest) _rest = false;
                if (!File.Exists(Path)) return;
                if (Block.Completed)
                {
                    Block.Downloading = false;
                    ThreadCompletedEvent?.Invoke(this);
                    return;
                }
                if (Block.From > Block.To)
                {
                    Block.Downloading = false;
                    Block.Completed = true;
                    if (!Info.DownloadCount.ContainsKey(DownloadUrl))
                        Info.DownloadCount.Add(DownloadUrl, 0);
                    Info.DownloadCount[DownloadUrl]++;
                    ThreadCompletedEvent?.Invoke(this);
                    return;
                }
                _request = WebRequest.CreateHttp(DownloadUrl);
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
                _response = (HttpWebResponse)_request.GetResponse();
                using (var responseStream = _response.GetResponseStream())
                {
                    using (
                        var stream = new FileStream(Path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite,
                            1024 * 1024))
                    {
                        stream.Seek(Block.From, SeekOrigin.Begin);
                        var array = new byte[1024];
                        var i = responseStream.Read(array, 0, array.Length);
                        while (true)
                        {
                            if (_stoped)
                            {
                                stream.Flush();
                                Block.Downloading = false;
                                return;
                            }
                            if (i <= 0 && Block.From - 1 != Block.To)
                            {
                                new Thread(Start) { IsBackground = true }.Start();
                                return;
                            }
                            if (i <= 0 || Block.From > Block.To) break;
                            stream.Write(BytesHandle(array), 0, i);
                            Block.From += i;
                            Block.CompletedLength += i;
                            Info.CompletedLength += i;
                            i = responseStream.Read(array, 0, array.Length);
                        }
                        stream.Flush();
                    }
                }
                Block.Completed = true;
                Block.Downloading = false;
                if (!Info.DownloadCount.ContainsKey(DownloadUrl))
                    Info.DownloadCount.Add(DownloadUrl, 0);
                Info.DownloadCount[DownloadUrl]++;
                ThreadCompletedEvent?.Invoke(this);
            }
            catch (WebException)
            {
                if (Block.From == Block.To)
                {
                    Block.Completed = true;
                    Block.Downloading = false;
                    ThreadCompletedEvent?.Invoke(this);
                    return;
                }
                Next();
                new Thread(Start) { IsBackground = true }.Start();
            }
            catch (Exception)
            {
                Next();
                new Thread(Start) { IsBackground = true }.Start();
            }
            finally
            {
                try
                {
                    _response?.Close();
                    _request?.Abort();
                }
                catch
                {
                }
            }
        }

        public virtual byte[] BytesHandle(byte[] bytes)
        {
            return bytes;
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            if (Block.Completed)
                return;
            _stoped = true;
        }

        public void Restart()
        {
        }

        public void ForceStop()
        {
            if (Block.Completed)
                return;
            _stoped = true;
            try
            {
                _response?.Close();
                _request?.Abort();
            }
            catch
            {
                // ignored
            }
            Block.Downloading = false;
        }

        private int _num;

        private void Next()
        {
            _num++;
            if (_num >= Info.DownloadUrl.Length) _num = 0;
            if (Info.DownloadUrl.Length == 0) return;
            DownloadUrl = Info.DownloadUrl[_num];
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BaiduPanDownloadWpf.Core.Download.DownloadCore.DownloadThread
{
    /// <summary>
    /// API专属下载代码
    /// </summary>
    internal class ApiDownloadThread : IDownloadThread
    {
        public int ID { get; set; }
        public string DownloadUrl { get; set; }
        public string Path { get; set; }
        public DownloadBlock Block { get; set; }
        public DownloadInfo Info { get; set; }

        public event Action ThreadCompletedEvent;

        bool _stoped;
        private HttpWebRequest _request;
        private HttpWebResponse _response;

        internal ApiDownloadThread()
        {
            //Task.Run(() => Start());
            new Thread(Start).Start();
        }

        private void Start()
        {
            //await Task.Delay(300);
            Thread.Sleep(300);
            while (true)
            {
                try
                {
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
                    if (Block.From > Block.To)
                    {
                        ThreadCompletedEvent?.Invoke();
                        return;
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
                    _request.ReadWriteTimeout = 3000;
                    _request.AddRange(Block.From, Block.To);
                    _response = _request.GetResponse() as HttpWebResponse;
                    if (!File.Exists(Path))
                    {
                        return;
                    }
                    using (var responseStream = _response.GetResponseStream())
                    {
                        using (var stream = new FileStream(Path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite, 1024*1024))
                        {
                            stream.Seek(Block.From, SeekOrigin.Begin);
                            var array = new byte[1024];
                            var i = responseStream.Read(array, 0, array.Length);
                            for (var j = 0; j < 25; j++)
                            {
                                if (_stoped)
                                {
                                    return;
                                }
                                if (i <= 0 && Block.From - 1 != Block.To && Block.From != Block.To)
                                {
                                    continue;
                                }
                                if (i <= 0)
                                {
                                    Block.Completed = true;
                                    break;
                                }
                                stream.Write(array, 0, i);
                                Block.From += i;
                                Block.CompletedLength += i;
                                Info.CompletedLength += i;
                                Info.DownloadBlockList[ID] = Block;
                                i = responseStream.Read(array, 0, array.Length);
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Error: "+ex);
                }
            }
        }

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
            throw new NotImplementedException();
        }
    }
}
